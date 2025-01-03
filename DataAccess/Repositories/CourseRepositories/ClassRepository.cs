﻿using EnglishCenter.Business.IServices;
using EnglishCenter.DataAccess.Database;
using EnglishCenter.DataAccess.Entities;
using EnglishCenter.DataAccess.IRepositories;
using EnglishCenter.Presentation.Global;
using EnglishCenter.Presentation.Global.Enum;
using EnglishCenter.Presentation.Helpers;
using EnglishCenter.Presentation.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenter.DataAccess.Repositories.CourseRepositories
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClaimService _claimService;

        public ClassRepository(
            EnglishCenterContext context,
            IWebHostEnvironment webHostEnvironment,
            IClaimService claimService) : base(context)
        {
            _webHostEnvironment = webHostEnvironment;
            _claimService = claimService;
        }

        public override IEnumerable<Class> GetAll()
        {
            var listClasses = context.Classes.Include(c => c.Teacher).ThenInclude(t => t.User).ToList();
            return listClasses;
        }
        public async Task<List<Class>?> GetClassesWithTeacherAsync(string teacherId)
        {
            if (teacherId == null) return null;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var classes = await context.Classes
                                    .Include(c => c.Teacher)
                                    .Where(c => c.TeacherId == teacherId &&
                                                c.Status != (int)ClassEnum.End &&
                                                c.StartDate <= currentDate && currentDate <= c.EndDate)
                                    .ToListAsync();
            return classes;
        }

        public async Task<bool> ChangeCourseAsync(Class model, string courseId)
        {
            if (model == null) return false;

            var course = await context.Courses.FindAsync(courseId);
            if (course == null) return false;

            model.CourseId = courseId;

            return true;
        }

        public Task<bool> ChangeEndTimeAsync(Class model, DateOnly endTime)
        {
            if (model == null) return Task.FromResult(false);

            if (model.StartDate.HasValue)
            {
                if (model.StartDate.Value > endTime)
                {
                    return Task.FromResult(false);
                }
            }

            model.EndDate = endTime;
            return Task.FromResult(true);
        }

        public async Task<bool> ChangeImageAsync(Class model, IFormFile image)
        {
            if (model == null) return false;

            var oldPathImage = Path.Combine(_webHostEnvironment.WebRootPath, model.Image ?? "");
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "classes", "images");
            var fileName = $"{DateTime.Now.Ticks}_{image.FileName}";

            var result = await UploadHelper.UploadFileAsync(image, uploadFolder, fileName);
            if (!string.IsNullOrEmpty(result)) return false;

            if (File.Exists(oldPathImage))
            {
                File.Delete(oldPathImage);
            }

            model.Image = Path.Combine("classes", "images", fileName);

            return true;
        }

        public Task<bool> ChangeMaxNumAsync(Class model, int maxNum)
        {
            if (model == null) return Task.FromResult(false);

            if (maxNum < 0)
            {
                return Task.FromResult(false);
            }

            model.MaxNum = maxNum;
            return Task.FromResult(true);
        }

        public Task<bool> ChangeStartTimeAsync(Class model, DateOnly startTime)
        {
            if (model == null) return Task.FromResult(false);

            if (model.EndDate.HasValue)
            {
                if (model.EndDate.Value < startTime)
                {
                    return Task.FromResult(false);
                }
            }

            model.StartDate = startTime;
            return Task.FromResult(true);
        }

        public Task<bool> ChangeDescriptionAsync(Class model, string newDes)
        {
            if (model == null) return Task.FromResult(false);

            model.Description = newDes;
            return Task.FromResult(true);
        }

        public async Task<bool> ChangeTeacherAsync(Class model, string teacherId)
        {
            if (model == null) return false;

            var isExistTeacher = await context.Teachers.AnyAsync(t => t.UserId == teacherId);
            if (!isExistTeacher) return false;

            var claimDto = new ClaimDto() { ClaimName = GlobalClaimNames.CLASS, ClaimValue = model.ClassId };
            var isSuccess = false;
            if (model.Status != (int)ClassEnum.End)
            {
                isSuccess = await _claimService.DeleteClaimInUserAsync(model.TeacherId, claimDto);
                if (!isSuccess) return false;
            }

            var isAddSuccess = await _claimService.AddClaimToUserAsync(teacherId, claimDto);
            if (!isSuccess) return false;

            model.TeacherId = teacherId;
            return true;
        }

        public async Task<bool> ChangeStatusAsync(Class model, ClassEnum status)
        {
            if (status == ClassEnum.End)
            {
                var isSuccess = await _claimService.DeleteClaimInUserAsync(model.TeacherId ?? "", new ClaimDto()
                {
                    ClaimName = GlobalClaimNames.CLASS,
                    ClaimValue = model.ClassId
                });

                if (!isSuccess) return isSuccess;
            }

            model.Status = (int)status;
            return true;
        }
    }
}
