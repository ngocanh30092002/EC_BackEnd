﻿using AutoMapper;
using EnglishCenter.Business.IServices;
using EnglishCenter.DataAccess.Entities;
using EnglishCenter.DataAccess.UnitOfWork;
using EnglishCenter.Presentation.Models;
using EnglishCenter.Presentation.Models.DTOs;
using EnglishCenter.Presentation.Models.ResDTOs;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenter.Business.Services.Courses
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unit;

        public AssignmentService(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        public async Task<Response> ChangeCourseContentAsync(long assignmentId, long contentId)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any assignments"
                };
            }

            var isExistCourseContent = _unit.CourseContents.IsExist(c => c.ContentId == contentId);
            if (!isExistCourseContent)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any course content"
                };
            }

            var isSuccess = await _unit.Assignments.ChangeCourseContentAsync(assignModel, contentId);
            if (!isSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't change course content",
                    Success = false,
                };
            }

            await _unit.CompleteAsync();
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "",
                Success = true
            };
        }

        public async Task<Response> ChangeNoNumAsync(long assignmentId, int number)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any assignments"
                };
            }

            if (number <= 0)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "NoNum must be greater than 0"
                };
            }

            var isSuccess = await _unit.Assignments.ChangeNoNumberAsync(assignModel, number);
            if (!isSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't change no number",
                    Success = false,
                };
            }

            await _unit.CompleteAsync();
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "",
                Success = true
            };
        }

        public async Task<Response> ChangePercentageAsync(long assignmentId, int percentage)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any assignments"
                };
            }

            if (percentage < 0 || percentage > 100)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Percentage must be between 0 - 100"
                };
            }

            var isSuccess = await _unit.Assignments.ChangePercentageAsync(assignModel, percentage);
            if (!isSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't change percentage",
                    Success = false,
                };
            }

            await _unit.CompleteAsync();
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "",
                Success = true
            };
        }

        public async Task<Response> ChangeTimeAsync(long assignmentId, string time)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any assignments"
                };
            }

            if (!TimeOnly.TryParse(time, out TimeOnly timeOnly))
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Time is not in correct format"
                };
            }


            var isSuccess = await _unit.Assignments.ChangeTimeAsync(assignModel, timeOnly);
            if (!isSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't change time",
                    Success = false,
                };
            }

            await _unit.CompleteAsync();
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "",
                Success = true
            };
        }

        public async Task<Response> ChangeTitleAsync(long assignmentId, string title)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Can't find any assignments"
                };
            }

            var isSuccess = await _unit.Assignments.ChangeTitleAsync(assignModel, title);
            if (!isSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't change title",
                    Success = false,
                };
            }

            await _unit.CompleteAsync();
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "",
                Success = true
            };
        }

        public async Task<Response> CreateAsync(AssignmentDto model)
        {
            var timeOnly = TimeOnly.MinValue;

            if (!string.IsNullOrEmpty(model.Time) && !TimeOnly.TryParse(model.Time, out timeOnly))
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Time is not in correct format"
                };
            }

            var courseContentModel = await _unit.CourseContents
                                                .Include(c => c.Assignments)
                                                .FirstOrDefaultAsync(c => c.ContentId == model.ContentId);

            if (courseContentModel == null)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't find any course contents"
                };
            }

            if (courseContentModel.Type != 1)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "It isn't possible to add courses with a status other than normal"
                };
            }

            var currentNum = courseContentModel.Assignments
                                            .Select(s => (int?)s.NoNum)
                                            .Max();

            var assignModel = new Assignment();
            assignModel.Title = model.Title;
            assignModel.Time = timeOnly;
            assignModel.CourseContentId = model.ContentId;
            assignModel.ExpectedTime = TimeOnly.MinValue;
            assignModel.AchievedPercentage = model.Achieved_Percentage;

            if (model.NoNum.HasValue)
            {
                if (model.NoNum.Value <= 0 || model.NoNum.Value > currentNum + 1)
                {

                    return new Response()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Message = "No num is invalid",
                        Success = false
                    };
                }

                var otherAssignments = _unit.Assignments
                                            .Find(s => s.CourseContentId == model.ContentId && s.NoNum >= model.NoNum.Value)
                                            .OrderBy(s => s.NoNum);

                foreach (var assign in otherAssignments)
                {
                    assign.NoNum = assign.NoNum + 1;
                }
                assignModel.NoNum = model.NoNum.Value;
            }
            else
            {
                assignModel.NoNum = currentNum.HasValue ? currentNum.Value + 1 : 1;
            }

            _unit.Assignments.Add(assignModel);
            await _unit.CompleteAsync();

            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = assignModel.AssignmentId,
                Success = true
            };
        }

        public async Task<Response> DeleteAsync(long assignmentId)
        {
            var assignModel = _unit.Assignments.GetById(assignmentId);
            if (assignModel == null)
            {
                return new Response()
                {
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't find any assignments"
                };
            }

            var maxNumber = _unit.Assignments
                                .Find(c => c.CourseContentId == assignModel.CourseContentId)
                                .Select(c => (int?)c.NoNum)
                                .Max();

            var isChangeSuccess = await _unit.Assignments.ChangeNoNumberAsync(assignModel, maxNumber ?? 1);
            if (!isChangeSuccess)
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't delete assignment",
                    Success = false
                };
            }

            _unit.Assignments.Remove(assignModel);
            await _unit.CompleteAsync();

            return new Response()
            {
                Success = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = ""
            };
        }

        public async Task<Response> GetAllAsync()
        {
            var assignments = _unit.Assignments.GetAll()
                                            .OrderBy(a => a.CourseContentId)
                                            .ThenBy(a => a.NoNum);

            foreach (var assign in assignments)
            {
                foreach (var item in assign.AssignQues)
                {
                    await _unit.AssignQues.LoadQuestionWithoutAnswerAsync(item);
                }
            }

            return new Response()
            {
                Success = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = _mapper.Map<List<AssignmentResDto>>(assignments)
            };
        }

        public async Task<Response> GetAsync(long assignmentId)
        {
            var assignment = _unit.Assignments
                                .Include(a => a.CourseContent)
                                .FirstOrDefault(a => a.AssignmentId == assignmentId);

            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = _mapper.Map<AssignmentResDto>(assignment),
                Success = true
            };
        }

        public async Task<Response> GetByCourseContentAsync(long courseContentId)
        {
            var assignments = await _unit.Assignments.GetByCourseContentAsync(courseContentId);

            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = _mapper.Map<List<AssignmentResDto>>(assignments),
                Success = true
            };
        }

        public async Task<Response> GetNumberByCourseAsync(string courseId)
        {
            if (!_unit.Courses.IsExist(c => c.CourseId == courseId))
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't find any courses",
                    Success = false
                };
            }

            var assignNum = await _unit.Assignments.GetNumberByCourseAsync(courseId);

            return new Response()
            {
                Message = assignNum,
                Success = true,
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<Response> GetTotalTimeByCourseAsync(string courseId)
        {
            if (!_unit.Courses.IsExist(c => c.CourseId == courseId))
            {
                return new Response()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Can't find any courses",
                    Success = false
                };
            }

            var totalTime = await _unit.Assignments.GetTotalTimeByCourseAsync(courseId);
            return new Response()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = totalTime,
                Success = true
            };
        }

        public async Task<Response> UpdateAsync(long assignmentId, AssignmentDto model)
        {
            var response = await _unit.Assignments.UpdateAsync(assignmentId, model);

            if (response.Success)
            {
                await _unit.CompleteAsync();
            }

            return response;
        }
    }
}
