using Edreams.OutlookMiddleware.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Model
{
    public class Log
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }
        public Guid? CorrelationId { get; set; }
        public string SourceContext { get; set; }
        public string MethodName { get; set; }
        public string SourceFile { get; set; }
        public string InsertedBy { get; set; }
        public int? LineNumber { get; set; }

    }
}
