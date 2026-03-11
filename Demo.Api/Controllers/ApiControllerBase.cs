namespace Demo.Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Wraps the Result object in either an OkObjectResult or BadRequestObjectResult 
    /// based on whether the result was successful.
    /// </summary>
    /// <typeparam name="T">Type of data that is returned in the Result object</typeparam>
    /// <param name="result">Result</param>
    /// <returns>IActionResult</returns>
    protected IActionResult ObjectResult<T>(Result<T> result)
    {
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Converts the result of the TSource to a Result of the TDestination and returns an OK result in the Result if successful for a BadResult if not.
    /// </summary>
    /// <typeparam name="TSource">Type of the source</typeparam>
    /// <typeparam name="TDestination">Type of the destination</typeparam>
    /// <param name="result">Result to convert</param>
    /// <returns>IActionResult</returns>
    protected IActionResult ObjectResult<TSource, TDestination>(Result<TSource> result)
    {
        if (!result.Succeeded || result.Data is null)
        {
            var failed = Result.Failed<TDestination>(result.Message);
            return BadRequest(failed);
        }

        var data = result.Data.Adapt<TDestination>();
        var success = Result.Success(data, result.Message);

        return Ok(success);
    }

    /// <summary>
    /// Maps the TSource to a TDestination using mapping and returns an OK result with the data
    /// </summary>
    /// <typeparam name="TSource">Type of the source</typeparam>
    /// <typeparam name="TDestination">Type of the destination</typeparam>
    /// <param name="source">Data to map</param>
    /// <returns>IActionResult</returns>
    protected IActionResult MapData<TSource, TDestination>(TSource source)
    {
        var data = source.Adapt<TDestination>();

        return Ok(data);
    }

    /// <summary>
    /// Returns a successful empty result
    /// </summary>
    /// <returns>IActionResult</returns>
    protected IActionResult SuccessfulEmptyResult(string message = "")
    {
        var result = Result.Success<Empty>(message);

        return Ok(result);
    }

    /// <summary>
    /// Returns a failed empty result
    /// </summary>
    /// <param name="message">Message to return</param>
    /// <returns>IActionResult</returns>
    protected IActionResult FailedEmptyResult(string message)
    {
        var result = Result.Failed<Empty>(message);

        return BadRequest(result);
    }
}
