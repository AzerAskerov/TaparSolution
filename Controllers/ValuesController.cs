using AWSServerless2.Models;
using Microsoft.AspNetCore.Mvc;

namespace AWSServerless2.Controllers;

[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
   

    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
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