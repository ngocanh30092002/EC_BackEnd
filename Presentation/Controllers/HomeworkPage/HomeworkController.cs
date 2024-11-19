﻿using System.Security.Claims;
using EnglishCenter.Business.IServices;
using EnglishCenter.Presentation.Global;
using EnglishCenter.Presentation.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishCenter.Presentation.Controllers.HomeworkPage
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeworkController : ControllerBase
    {
        private readonly IHomeworkService _homeService;

        public HomeworkController(IHomeworkService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _homeService.GetAllAsync();
            return await response.ChangeActionAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] long id)
        {
            var response = await _homeService.GetAsync(id);
            return await response.ChangeActionAsync();
        }

        [HttpPost]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> CreateAsync([FromForm] HomeworkDto model)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);

            if(isTeacher)
            {
                var isValid = User.Claims.Any(c => c.Type == GlobalClaimNames.CLASS && c.Value == model.ClassId);
                if (!isValid)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.CreateAsync(model);
            return await response.ChangeActionAsync();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> UpdateAsync([FromRoute] long id ,[FromForm] HomeworkDto model)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);

            if (isTeacher)
            {
                var isValid = User.Claims.Any(c => c.Type == GlobalClaimNames.CLASS && c.Value == model.ClassId);
                if (!isValid)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.UpdateAsync(id, model);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-start-time")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeStartTimeAsync([FromRoute] long id, [FromBody] string startTime)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangeStartTimeAsync(id, startTime);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-end-time")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeEndTimeAsync([FromRoute] long id, [FromBody] string endTime)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangeEndTimeAsync(id, endTime);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-late-days")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeLateSubmitDaysAsync([FromRoute] long id, [FromQuery] int days)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangeLateSubmitDaysAsync(id, days);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-percentage")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangePercentageAsync([FromRoute] long id, [FromQuery] int percentage)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangePercentageAsync(id, percentage);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-time")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeTimeAsync([FromRoute] long id, [FromBody] string time)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangeTimeAsync(id, time);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{id}/change-title")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeTitleAsync([FromRoute] long id, [FromBody] string title)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.ChangeTimeAsync(id, title);
            return await response.ChangeActionAsync();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> DeleteAsync([FromRoute] long id)
        {
            var isTeacher = User.IsInRole(AppRole.TEACHER);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (isTeacher)
            {
                var isInCharge = await _homeService.IsInChargeClass(userId, id);
                if (!isInCharge)
                {
                    return Forbid("You aren't in charge of this class so you can't access it.");
                }
            }

            var response = await _homeService.DeleteAsync(id);
            return await response.ChangeActionAsync();
        }
    }
}
