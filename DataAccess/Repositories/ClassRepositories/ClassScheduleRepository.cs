﻿using EnglishCenter.DataAccess.Database;
using EnglishCenter.DataAccess.Entities;
using EnglishCenter.DataAccess.IRepositories;
using EnglishCenter.Presentation.Global.Enum;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenter.DataAccess.Repositories.ClassRepositories
{
    public class ClassScheduleRepository : GenericRepository<ClassSchedule>, IClassScheduleRepository
    {
        public ClassScheduleRepository(EnglishCenterContext context) : base(context)
        {

        }

        public async Task<bool> ChangeClassAsync(ClassSchedule schedule, string classId)
        {
            if (schedule == null) return false;

            var classModel = await context.Classes.FindAsync(classId);
            if (classModel == null) return false;

            var isExist = context.ClassSchedules.Where(s => s.ClassId == classId && s.DayOfWeek == schedule.DayOfWeek && s.IsActive == true).Any();
            if (isExist) return false;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (classModel.Status == (int)ClassEnum.End || (classModel.StartDate >= currentDate || currentDate >= classModel.EndDate))
            {
                schedule.IsActive = false;
                schedule.ClassId = classId;
                return true;
            }

            schedule.IsActive = true;
            schedule.ClassId = classId;
            return true;
        }

        public async Task<bool> ChangeClassRoomAsync(ClassSchedule schedule, long classRoomId)
        {
            if (schedule == null) return false;

            var classRoomModel = await context.ClassRooms.FindAsync(classRoomId);
            if (classRoomModel == null) return false;
            if (classRoomModel.Capacity < schedule.Class.RegisteredNum) return false;

            if (schedule.IsActive == false)
            {
                schedule.ClassRoomId = classRoomId;
                return true;
            }

            bool isDuplicate = await IsDuplicateAsync(schedule.DayOfWeek, classRoomId, schedule.StartPeriod, schedule.EndPeriod);
            if (isDuplicate) return false;
            schedule.ClassRoomId = classRoomId;
            return true;
        }

        public async Task<bool> ChangeDayOfWeekAsync(ClassSchedule schedule, int dayOfWeek)
        {
            if (schedule == null) return false;

            if (dayOfWeek < 0 || dayOfWeek >= 7) return false;

            if (schedule.IsActive == false)
            {
                schedule.DayOfWeek = dayOfWeek;
                return true;
            }

            var isExist = context.ClassSchedules.Where(s => s.ClassId == schedule.ClassId && s.DayOfWeek == dayOfWeek && s.IsActive == true).Any();
            if (isExist) return false;

            bool isDuplicate = await IsDuplicateAsync(dayOfWeek, schedule.ClassRoomId, schedule.StartPeriod, schedule.EndPeriod);
            if (isDuplicate) return false;

            schedule.DayOfWeek = dayOfWeek;

            return true;
        }

        public async Task<bool> ChangeEndPeriodAsync(ClassSchedule schedule, int end)
        {
            if (schedule == null) return false;

            if (schedule.StartPeriod > end) return false;
            if (end <= 0 || end >= 13) return false;

            if (schedule.IsActive == false)
            {
                schedule.EndPeriod = end;
                return true;
            }

            bool isDuplicate = await IsDuplicateAsync(schedule.DayOfWeek, schedule.ClassRoomId, schedule.StartPeriod, end, schedule.ScheduleId);
            if (isDuplicate) return false;

            schedule.EndPeriod = end;

            return true;
        }

        public async Task<bool> ChangeStartPeriodAsync(ClassSchedule schedule, int start)
        {
            if (schedule == null) return false;

            if (start > schedule.EndPeriod) return false;
            if (start <= 0 || start >= 13) return false;

            if (schedule.IsActive == false)
            {
                schedule.StartPeriod = start;
                return true;
            }

            bool isDuplicate = await IsDuplicateAsync(schedule.DayOfWeek, schedule.ClassRoomId, start, schedule.EndPeriod, schedule.ScheduleId);
            if (isDuplicate) return false;

            schedule.StartPeriod = start;

            return true;
        }

        public async Task<bool> IsDuplicateAsync(int dayOfWeek, long classRoomId, int start, int end, long? scheduleId = null)
        {
            var schedules = new List<ClassSchedule>();

            if (scheduleId.HasValue)
            {
                schedules = await context.ClassSchedules
                                  .Where(c => c.DayOfWeek == dayOfWeek &&
                                              c.ClassRoomId == classRoomId &&
                                              c.IsActive == true &&
                                              c.ScheduleId != scheduleId.Value)
                                  .ToListAsync();
            }
            else
            {
                schedules = await context.ClassSchedules
                                   .Where(c => c.DayOfWeek == dayOfWeek &&
                                               c.ClassRoomId == classRoomId &&
                                               c.IsActive == true)
                                   .ToListAsync();
            }

            if (start > end) return true;

            foreach (var schedule in schedules)
            {
                if (schedule.StartPeriod <= start && start <= schedule.EndPeriod)
                {
                    return true;
                }
                if (schedule.StartPeriod <= end && end <= schedule.EndPeriod)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
