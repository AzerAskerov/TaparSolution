using TaparSolution.Models;
using Microsoft.AspNetCore.Mvc;

namespace TaparSolution.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
   

    // GET api/values/5
    [HttpGet("{pin}")]
    public string Get(string pin)
    {
        return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post()
    {
       
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}