
using TaparSolution.Helpers;
using TaparSolution.Models;

namespace TaparSolution.Operations
{
    public abstract class BaseOperation<T> : IDisposable where T : BaseOperationModel
    {
        public T Parameter { get; set; }
      public  OperationResult Result = new OperationResult();
        public BaseOperation()
        {
           
        }
        public virtual void Validate()
        { }

        public virtual async void DoFinalize() {
            if (!Result.IsSuccess)
            await TelegramMessageComposerHelper.SendInfoToAdmin(Result.ToString());
        }

        public  abstract void DoExecute() ;
        public  Task<OperationResult> ExecuteAsync(T param)
        {
            Parameter = param;

           

             Task<OperationResult> t = new(() => { 
            try
            {
                Validate();
                if (Result.IsSuccess)
                {
                    DoExecute();
                }
                return Result;
            }
            catch (Exception ex)
            {
                Result.AddError(ex.Message);

            }
                 finally { DoFinalize(); }  
                 
            return Result;

            });
            t.Start();
            return t;
        }

        public void Dispose()
        {
           
        }

       
    }

    public abstract class BaseOperationModel
    {
        public  DynamoDbClient _db { get { return DynamoDbClient.GetInstance(); } }
    }

    public class OperationResult
    {
        public override string ToString()
        {
            return String.Join(';', Issues.Select(x=>x.Message));
        }
        public List<Issue> Issues = new List<Issue>();

        public bool IsSuccess
        {
            get
            {
                return
                        !Issues.Any(x => x.Severity == SeverityEnum.Error);
            }
        }

        public void AddError(string _message)
        {
            this.Issues.Add(new Issue() { Severity = SeverityEnum.Error, Message = _message });
        }

        public void AddInformation(string _message)
        {
            this.Issues.Add(new Issue() { Severity = SeverityEnum.Information, Message = _message });
        }

        public void AddWarning(string _message)
        {
            this.Issues.Add(new Issue() { Severity = SeverityEnum.Warning, Message = _message });
        }
    }
    public class Issue
    {
        public SeverityEnum Severity { get; set; }
        public string? Message { get; set; }

    }





    public enum SeverityEnum
    {
        Success = 1,
        Warning = 2,
        Information = 3,
        Error = 4
    }
}