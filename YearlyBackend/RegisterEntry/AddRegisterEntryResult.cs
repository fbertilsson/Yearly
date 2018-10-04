namespace YearlyBackend.RegisterEntry
{
  public class AddRegisterEntryResult
    {
        public bool ValidatedOk { get; }
        public string ValidationError { get; }

        public AddRegisterEntryResult(bool validatedOk = true, string validationError = "")
        {
            ValidatedOk = validatedOk;
            ValidationError = validationError;
        }
    }
}
