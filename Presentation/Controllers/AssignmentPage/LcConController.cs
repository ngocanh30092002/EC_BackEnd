﻿using EnglishCenter.Business.IServices;
using EnglishCenter.Presentation.Global;
using EnglishCenter.Presentation.Helpers;
using EnglishCenter.Presentation.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EnglishCenter.Presentation.Controllers.AssignmentPage
{
    [Route("api/lc-con")]
    [ApiController]
    [Authorize]
    public class LcConController : ControllerBase
    {
        private readonly IQuesLcConService _queLcConService;
        private readonly IAnswerLcConService _answerService;
        private readonly ISubLcConService _subLcConService;

        public LcConController(IQuesLcConService queLcConService, IAnswerLcConService answerService, ISubLcConService subLcConService)
        {
            _queLcConService = queLcConService;
            _answerService = answerService;
            _subLcConService = subLcConService;
        }

        #region Question

        [HttpGet]
        public async Task<IActionResult> GetQuesConversationsAsync()
        {
            var response = await _queLcConService.GetAllAsync();
            return await response.ChangeActionAsync();
        }

        [HttpGet("assignments/{id}/other")]
        public async Task<IActionResult> GetOtherQuestionByAssignmentAsync([FromRoute] long id)
        {
            var response = await _queLcConService.GetOtherQuestionByAssignmentAsync(id);
            return await response.ChangeActionAsync();
        }

        [HttpGet("homework/{id}/other")]
        public async Task<IActionResult> GetOtherQuestionByHomeworkAsync([FromRoute] long id)
        {
            var response = await _queLcConService.GetOtherQuestionByHomeworkAsync(id);
            return await response.ChangeActionAsync();
        }

        [HttpGet("{quesId}")]
        public async Task<IActionResult> GetQuesConversationAsync([FromRoute] long quesId)
        {
            var response = await _queLcConService.GetAsync(quesId);
            return await response.ChangeActionAsync();
        }

        [HttpPost]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> CreateAsync([FromForm] QuesLcConDto queModel)
        {
            if (queModel.Image != null)
            {
                var isImageFile = await UploadHelper.IsImageAsync(queModel.Image);
                if (!isImageFile)
                {
                    return BadRequest(new { message = "The image file is invalid. Only JPEG, PNG, GIF, and SVG are allowed.", success = false });
                }
            }

            if (queModel.Audio != null)
            {
                var isAudioFile = await UploadHelper.IsAudioAsync(queModel.Audio);
                if (!isAudioFile)
                {
                    return BadRequest(new { message = "The audio file is invalid. Only MP3, WAV and OGG are allowed. ", success = false });
                }
            }

            if (!string.IsNullOrEmpty(queModel.SubLcConsJson))
            {
                queModel.SubLcCons = JsonConvert.DeserializeObject<List<SubLcConDto>>(queModel.SubLcConsJson);
            }


            var response = await _queLcConService.CreateAsync(queModel);
            return await response.ChangeActionAsync();
        }

        [HttpPut("{quesId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> UpdateAsync([FromRoute] long quesId, [FromForm] QuesLcConDto queModel)
        {
            if (queModel.Image != null)
            {
                var isImageFile = await UploadHelper.IsImageAsync(queModel.Image);
                if (!isImageFile)
                {
                    return BadRequest(new { message = "The image file is invalid. Only JPEG, PNG, GIF, and SVG are allowed.", success = false });
                }
            }

            if (queModel.Audio != null)
            {
                var isAudioFile = await UploadHelper.IsAudioAsync(queModel.Audio);
                if (!isAudioFile)
                {
                    return BadRequest(new { message = "The audio file is invalid. Only MP3, WAV and OGG are allowed. ", success = false });
                }
            }

            var response = await _queLcConService.UpdateAsync(quesId, queModel);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{quesId}/image")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeImageAsync([FromRoute] long quesId, IFormFile imageFile)
        {
            var isImageFile = await UploadHelper.IsImageAsync(imageFile);
            if (!isImageFile)
            {
                return BadRequest(new { message = "The image file is invalid. Only JPEG, PNG, GIF, and SVG are allowed.", success = false });
            }

            var response = await _queLcConService.ChangeImageAsync(quesId, imageFile);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{quesId}/audio")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeAudioAsync([FromRoute] long quesId, IFormFile audioFile)
        {
            var isAudioFile = await UploadHelper.IsAudioAsync(audioFile);
            if (!isAudioFile)
            {
                return BadRequest(new { message = "The audio file is invalid. Only MP3, WAV and OGG are allowed. ", success = false });
            }

            var response = await _queLcConService.ChangeAudioAsync(quesId, audioFile);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("{quesId}/quantity")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeQuantityAsync([FromRoute] long quesId, [FromQuery] int quantity)
        {
            var response = await _queLcConService.ChangeQuantityAsync(quesId, quantity);
            return await response.ChangeActionAsync();
        }

        [HttpDelete("{quesId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> DeleteAsync([FromRoute] long quesId)
        {
            var response = await _queLcConService.DeleteAsync(quesId);
            return await response.ChangeActionAsync();
        }
        #endregion

        #region SubQuestion
        [HttpGet("subs")]
        public async Task<IActionResult> GetSubsAsync()
        {
            var response = await _subLcConService.GetAllAsync();
            return await response.ChangeActionAsync();
        }

        [HttpGet("subs/{subId}")]
        public async Task<IActionResult> GetSubAsync([FromRoute] long subId)
        {
            var response = await _subLcConService.GetAsync(subId);
            return await response.ChangeActionAsync();
        }

        [HttpPost("subs")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> CreateSubAsync([FromForm] SubLcConDto queModel)
        {
            var response = await _subLcConService.CreateAsync(queModel);
            return await response.ChangeActionAsync();
        }

        [HttpPut("subs/{subId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> UpdateAsync([FromRoute] long subId, [FromForm] SubLcConDto queModel)
        {
            var response = await _subLcConService.UpdateAsync(subId, queModel);
            return await response.ChangeActionAsync();
        }


        [HttpPatch("subs/{subId}/answerA")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubAnswerAAsync([FromRoute] long subId, [FromBody] string newAnswer)
        {
            var response = await _subLcConService.ChangeAnswerAAsync(subId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/answerB")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubAnswerBAsync([FromRoute] long subId, [FromBody] string newAnswer)
        {
            var response = await _subLcConService.ChangeAnswerBAsync(subId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/answerC")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubAnswerCAsync([FromRoute] long subId, [FromBody] string newAnswer)
        {
            var response = await _subLcConService.ChangeAnswerCAsync(subId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/answerD")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubAnswerDAsync([FromRoute] long subId, [FromBody] string newAnswer)
        {
            var response = await _subLcConService.ChangeAnswerDAsync(subId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/answer")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubAnswerAsync([FromRoute] long subId, [FromBody] int answerId)
        {
            var response = await _subLcConService.ChangeAnswerAsync(subId, answerId);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/question")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubQuestionAsync([FromRoute] long subId, [FromBody] string newQues)
        {
            var response = await _subLcConService.ChangeQuestionAsync(subId, newQues);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/no-num")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubNoNumAsync([FromRoute] long subId, [FromBody] int noNum)
        {
            var response = await _subLcConService.ChangeNoNumAsync(subId, noNum);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("subs/{subId}/pre-ques")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeSubPreQuesAsync([FromRoute] long subId, [FromBody] long preQues)
        {
            var response = await _subLcConService.ChangePreQuesAsync(subId, preQues);
            return await response.ChangeActionAsync();
        }

        [HttpDelete("subs/{subId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> DeleteSubAsync([FromRoute] long subId)
        {
            var response = await _subLcConService.DeleteAsync(subId);
            return await response.ChangeActionAsync();
        }


        #endregion

        #region Answer

        [HttpGet("answers")]
        public async Task<IActionResult> GetAnswersAsync()
        {
            var response = await _answerService.GetAllAsync();
            return await response.ChangeActionAsync();
        }

        [HttpGet("answer/{answerId}")]
        public async Task<IActionResult> GetAnswerAsync([FromRoute] long answerId)
        {
            var response = await _answerService.GetAsync(answerId);
            return await response.ChangeActionAsync();
        }

        [HttpPost("answers")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> CreateAnswerAsync([FromForm] AnswerLcConDto model)
        {
            var response = await _answerService.CreateAsync(model);
            return await response.ChangeActionAsync();
        }

        [HttpPut("answers/{answerId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> UpdateAnswerAsync([FromRoute] long answerId, [FromForm] AnswerLcConDto model)
        {
            var response = await _answerService.UpdateAsync(answerId, model);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/answerA")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeAnswerAAsync([FromRoute] long answerId, [FromBody] string newAnswer)
        {
            var response = await _answerService.ChangeAnswerAAsync(answerId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/answerB")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeAnswerBAsync([FromRoute] long answerId, [FromBody] string newAnswer)
        {
            var response = await _answerService.ChangeAnswerBAsync(answerId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/answerC")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeAnswerCAsync([FromRoute] long answerId, [FromBody] string newAnswer)
        {
            var response = await _answerService.ChangeAnswerCAsync(answerId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/answerD")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeAnswerDAsync([FromRoute] long answerId, [FromBody] string newAnswer)
        {
            var response = await _answerService.ChangeAnswerDAsync(answerId, newAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/correctAnswer")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeCorrectAnswerAsync([FromRoute] long answerId, [FromBody] string newCorrectAnswer)
        {
            var response = await _answerService.ChangeCorrectAnswerAsync(answerId, newCorrectAnswer);
            return await response.ChangeActionAsync();
        }

        [HttpPatch("answers/{answerId}/question")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> ChangeQuestionAsync([FromRoute] long answerId, [FromBody] string newQues)
        {
            var response = await _answerService.ChangeQuestionAsync(answerId, newQues);
            return await response.ChangeActionAsync();
        }

        [HttpDelete("answers/{answerId}")]
        [Authorize(Policy = GlobalVariable.ADMIN_TEACHER)]
        public async Task<IActionResult> DeleteAnswerAsync([FromRoute] long answerId)
        {
            var response = await _answerService.DeleteAsync(answerId);
            return await response.ChangeActionAsync();
        }
        #endregion
    }
}
