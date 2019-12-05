using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
   [Table("SodLogging")]
   public  class LoggingModels
    {
   
      /// <summary>
     /// Columns Name are created as per table Column Name schema
     /// </summary>

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogId                    {get;set;}
	public DateTime LogDate             {get;set;}  
	public string LogMessage            {get;set;}  
	public string LogData               {get;set;}  
	public string LogSource             {get;set;}  
	public string HelpLink              {get;set;}  
	public string HResult               {get;set;}  
	public string InnerException        {get;set;}
    public string MethodName            {get; set;}
    public string FilePath              {get; set;}
    public int EmpId                    {get; set;}  
   
   }


   [Table("SmsLog")]
   public class SmsLoggingModels
   { 

       /// <summary>
       /// Columns Name are created as per table Column Name schema
       /// </summary>

       [Key]
       public int LogId { get; set; }
       public DateTime LogDate { get; set; }
       public string LogMessage { get; set; }
       public string LogData { get; set; }
       public string LogSource { get; set; }
       public string HelpLink { get; set; }
       public string HResult { get; set; }
       public string InnerException { get; set; }
       public string MethodName { get; set; }
       public string FilePath { get; set; }
       
   }
}
