namespace AmbalajliyoMVC.ViewModels
{ /// <summary>
  /// ViewModel for displaying log entries in the Ambalajliyo MVC application.
  /// </summary>
    public class LogEntryViewModel
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
    }
}
