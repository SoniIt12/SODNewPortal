using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SOD.Model;
using System.Web;
using System.IO;
using SOD;
using System.Text;


namespace SOD.Services.ADO
{
    public class SodCommonServices
    {

      

        /// <summary>
        /// Get Department Info
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<DepartmentModels> GetDepartmentList(int Id, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstdept = new List<DepartmentModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodCommon"
                };
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstdept.Add(new DepartmentModels());
                        lstdept[lstdept.Count - 1].id = Convert.ToInt32(dr["Id"].ToString());
                        lstdept[lstdept.Count - 1].dept_name = dr["dept_name"].ToString();
                        lstdept[lstdept.Count - 1].dept_isactive = Convert.ToInt16(dr["dept_is_active"].ToString());
                        // lstdept[lstdept.Count - 1].vertical_name = dr["vertical_name"].ToString();
                        // lstdept[lstdept.Count - 1].vertical_id = dr["vertical_id"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstdept;
        }

        /// <summary>
        /// Get Department Info
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<DepartmentModels> GetDepartmentVerticalList(int Id, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstdept = new List<DepartmentModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodCommon"
                };
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstdept.Add(new DepartmentModels());
                        lstdept[lstdept.Count - 1].id = Convert.ToInt32(dr["Id"].ToString());
                        lstdept[lstdept.Count - 1].dept_name = dr["dept_name"].ToString();
                        lstdept[lstdept.Count - 1].dept_isactive = Convert.ToInt16(dr["dept_is_active"].ToString());
                        lstdept[lstdept.Count - 1].vertical_name = dr["vertical_name"].ToString();
                        lstdept[lstdept.Count - 1].vertical_id = dr["vertical_id"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstdept;
        }
        /// <summary>
        /// Get Department Info
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<VerticalModels> GetDeptVerticals(int Id, int criteria)
        {

            var strConnString = ConnectionUtility.GetConnection();
            var lstVert = new List<VerticalModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodCommon"
                };
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstVert.Add(new VerticalModels());
                        lstVert[lstVert.Count - 1].DepartmentId = Convert.ToInt32(dr["DepartmentID"].ToString());
                        lstVert[lstVert.Count - 1].VerticalID = dr["VerticalID"].ToString();
                        lstVert[lstVert.Count - 1].VerticalName = dr["VerticalName"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstVert;
        }
        /// <summary>
        /// Get Employee Designation List
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<DesignationModels> GetDesignationList(int Id, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstdesig = new List<DesignationModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodCommon"
                };
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstdesig.Add(new DesignationModels());
                        lstdesig[lstdesig.Count - 1].id = Convert.ToInt32(dr["Id"].ToString());
                        lstdesig[lstdesig.Count - 1].designation_name = dr["designation_name"].ToString();
                        lstdesig[lstdesig.Count - 1].designation_level = Convert.ToInt32(dr["designation_level"].ToString());
                        //lstdesig[lstdesig.Count - 1].designation_level = dr["designation_level"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstdesig;
        }
        /// <summary>
        /// Get Employee Id
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static int GetLoginEmployeeID(string empCode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var empId = 0;
            try
            {
                using (var con = new SqlConnection(strConnString))
                {
                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "SP_SodGetEmpId"
                    };
                    cmd.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = empCode;
                    cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                    cmd.Connection = con;
                    try
                    {
                        con.Open();
                        empId = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                string path = HttpContext.Current.Server.MapPath("~/ErrorLog/Log.txt");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
            return empId;
        }
        /// <summary>
        /// Get Employee Common Information
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetEmployeeCommonDetails(int empId)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodEmployeeCommonInfo"
                };
                cmd.Parameters.Add("@empId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 1;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Department"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Designation = dr["Designation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phone = dr["Phone"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentId = Convert.ToInt32(dr["DepartmentId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationId = Convert.ToInt32(dr["DesignationId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["EmployeeVertical"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVerticleName = dr["EmployeeVerticleName"].ToString();

                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }

        public static List<EmployeeModel> GetEmployeeCommonDetailsSJSC(string EmailId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodEmployeeCommonInfoSJSC"
                };
                cmd.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = EmailId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Department"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Designation = dr["Designation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phone = dr["Phone"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentId = 0;
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationId = 0;
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["EmployeeVertical"].ToString(); ;
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVerticleName = "0";

                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }


        public static List<EmployeeModel> NonSODUserRoleDetails()
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_GetNonSODUserRoleDetails"
                };
                cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = 7;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 1;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmployeeCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["Name"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }
        /// <summary>
        /// Bulk :Get Employee Common Information by Transaction Id
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetBulkBookingUserInfoByTransactionId(Int64 TrRequestId, string empCode, int Criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_BulkBookingUserInfoByTransactionId"
                };
                cmd.Parameters.Add("@TrId", SqlDbType.Int).Value = TrRequestId;
                cmd.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = empCode;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = Criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Department"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Designation = dr["Designation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phone = dr["Phone"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentId = Convert.ToInt32(dr["DepartmentId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationId = Convert.ToInt32(dr["DesignationId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["EmployeeVertical"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVerticleName = dr["EmployeeVerticleName"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }

        /// <summary>
        /// Get Employee Common Information
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetEmployeeCommonInfo(string empCode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodEmployeeCommonInfo"
                };
                cmd.Parameters.Add("@EmpName", SqlDbType.VarChar).Value = empCode;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;

                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Department"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Designation = dr["Designation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phone = dr["Phone"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentId = Convert.ToInt32(dr["DepartmentId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationId = Convert.ToInt32(dr["DesignationId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["EmployeeVertical"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVerticleName = dr["EmployeeVerticleName"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }

        /// <summary>
        /// Get Sod Blanket Approval Information
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetSodBlanketApprovals(int? deptId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodApprover"
                };
                cmd.Parameters.Add("@deptId", SqlDbType.Int).Value = deptId;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Dept_Name"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Active = Convert.ToBoolean(dr["IsActive"]);
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }


        /// <summary>
        /// Get Sod Approval Information
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<EmployeeModel> GetSodApprovals(int? deptId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<EmployeeModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodApprover"
                };
                cmd.Parameters.Add("@deptId", SqlDbType.Int).Value = deptId;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].SodApproverID = Convert.ToInt32(dr["SodApproverId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpName = dr["EmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Department = dr["Dept_Name"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Active = Convert.ToBoolean(dr["IsActive"]);
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationId = Convert.ToInt32(dr["DesignationId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentId = Convert.ToInt32(dr["DepartmentId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["VerticalID"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVerticleName = dr["VerticalName"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstEmployeeList;
        }

        /// <summary>
        /// Get Sod Booking Info for Approval 
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<TravelRequestModels> GetSodBookingListForApproval(int deptartmentId, int designationId, int empId, int criteria, string HodEmailId)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingList"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@DesignationId", SqlDbType.Int).Value = designationId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Parameters.Add("@hodemailId", SqlDbType.VarChar).Value = HodEmailId;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpCode = dr["RequestedEmpCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingType"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingInfoList;
        }


        /// <summary>
        /// Get Sod Booking Info for Approval 
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<OALTravelRequestMasterModel> GetOatBookingListForApproval(int deptartmentId, int designationId, int empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<OALTravelRequestMasterModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_OatApproverBookingList"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@DesignationId", SqlDbType.Int).Value = designationId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new OALTravelRequestMasterModel());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestDate = Convert.ToDateTime(dr["RequestDate"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();

                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingInfoList;
        }


        /// <summary>
        /// Get Sod Employee Booking History  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<TravelRequestModels> GetSodEmployeeBookingHistoryList(int? deptartmentId, int? empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingListEmpHistory"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingInfoList;
        }


        public static List<TravelRequestModels> GetSodEmployeeBookingHistoryListbydate(DateTime fromdate, DateTime todate, Int32 EmpId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingReportEmpHistory"
                };
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromdate;// "02/01/2019 12:00:00 AM";
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = todate;// "08/07/2019 12:00:00 AM";
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = EmpId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingInfoList;
        }
        /// <summary>
        /// Get Sod Emloyee Booking Status
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> GetEmployeeBookingStatus(int reqId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingStatus = new List<string>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingListEmpStatus"
                };
                cmd.Parameters.Add("@ReqId", SqlDbType.Int).Value = reqId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingStatus.Add(dr["Comment"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingStatus;
        }


        /// <summary>
        /// Check Duplicate PNR Status
        /// </summary>
        /// <param name="TrRequestID"></param>
        /// <returns></returns>
        public static string CheckDuplicatePNR(Int64 TrRequestID, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var pnr = string.Empty;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodCheckDuplicatePNR"
                };
                cmd.Parameters.Add("@TrRequestID", SqlDbType.BigInt).Value = TrRequestID;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    pnr = cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return pnr;
        }

        public static string CheckDuplicatePNR_edit(Int64 TrRequestID, int criteria, Int32 AddNo, Int32 BreqID)
        {

            var strConnString = ConnectionUtility.GetConnection();
            var pnr = string.Empty;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodCheckDuplicatePNR_Edit"
                };
                cmd.Parameters.Add("@TrRequestID", SqlDbType.Int).Value = TrRequestID;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Parameters.Add("@AddNo", SqlDbType.Int).Value = AddNo;
                cmd.Parameters.Add("@BreqId", SqlDbType.Int).Value = BreqID;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    pnr = cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return pnr;
        }



        /// <summary>
        /// Geet HOD Name, EMail and EmpID
        /// </summary>
        /// <param name="verticalId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static string GetHodEmailDetails(string verticalId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var details = string.Empty;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetHOD"
                };
                cmd.Parameters.Add("@verticalId", SqlDbType.VarChar).Value = verticalId;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    details = cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return details;   //by 
        }


        /// <summary>
        /// Geet HOD Name, EMail and EmpID for Bulk Booking
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="Criteria"></param>
        /// <returns></returns>
        public static string GetHodEmailDetailsBulkBooking(Int64 TravelRequestId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var details = string.Empty;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetHODBulkBooking"
                };
                cmd.Parameters.Add("@TravelRequestId", SqlDbType.BigInt).Value = TravelRequestId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    details = cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return details;   //by Soni
        }
        /// <summary>
        /// Get Sod Employee Booking History  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk(int? deptartmentId, int? empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var lstDept = new List<DepartmentModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingListEmpHistory"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestTypeId = Convert.ToInt16(dr["TravelRequestTypeId"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsVendorBooking = dr["IsVendorBooking"] is DBNull ? false : Convert.ToBoolean(dr["IsVendorBooking"].ToString());
                    }
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            lstDept.Add(new DepartmentModels());
                            lstDept[lstDept.Count - 1].id = Convert.ToInt32(dr["department_id"].ToString());
                            lstDept[lstDept.Count - 1].dept_name = dr["dept_name"].ToString();
                        }
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("bookingList", lstBookingInfoList);
                dicList.Add("deptList", lstDept);
            }
            return dicList;
        }
        /// <summary>
        /// Get Employee Data
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeViewDetails(string empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmpDetails = new List<EmployeeViewDetails>();

            var dicList = new Dictionary<string, object>();
            var lstEmployeeList = new List<EmployeeViewDetails>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodEmployeeCommonInfo"
                };
                cmd.Parameters.Add("@empFilter", SqlDbType.VarChar).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 3;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new EmployeeViewDetails());
                        string date_of_birth = dr["DOB"].ToString().Remove(10);
                        string dob = date_of_birth.Substring(8, 2) + "/" + date_of_birth.Substring(5, 2) + "/" + date_of_birth.Substring(0, 4);

                        lstEmployeeList[lstEmployeeList.Count - 1].EntryID = Convert.ToInt32(dr["EntryID"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].ID = Convert.ToInt32(dr["ID"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeCode = dr["EmployeeCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].FirstName = dr["FirstName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].LastName = dr["LastName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DOB = dob;
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentID = Convert.ToInt32(dr["DepartmentID"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DepartmentName = dr["DepartmentName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationID = Convert.ToInt32(dr["DesignationID"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].DesignationName = dr["DesignationName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phone = dr["Phone"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Email = dr["Email"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmployeeVertical = dr["EmployeeVertical"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmpVerticalDescription = dr["EmpVerticalDescription"].ToString();
                    }
                    dicList.Add("emplist", lstEmployeeList);

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            dicList.Add("totalEmpList", dr["empCount"].ToString());
                        }
                    }

                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

            return dicList;

        }
        /// <summary>
        /// Get Sod Employee Booking History for Export to Excel Data
        /// </summary>
        /// <param name="fdate"></param>
        /// <param name="tdate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(string fdate, string tdate, short type, string EmpId, string Dept, short criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<ExcelExportModel>();
            var dicList = new Dictionary<string, object>();
            try
            {
                using (var con = new SqlConnection(strConnString))
                {
                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "SP_SodBookingListEmpHistory_Export"
                    };
                    cmd.Parameters.Add("@fdate", SqlDbType.VarChar, 10).Value = fdate;
                    cmd.Parameters.Add("@tdate", SqlDbType.VarChar, 10).Value = tdate;
                    cmd.Parameters.Add("@type", SqlDbType.SmallInt).Value = type;
                    cmd.Parameters.Add("@dept", SqlDbType.VarChar).Value = Dept;
                    cmd.Parameters.Add("@EmpId", SqlDbType.VarChar).Value = EmpId;
                    cmd.Parameters.Add("@criteria", SqlDbType.SmallInt).Value = criteria;
                    cmd.Connection = con;
                    try
                    {
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {

                            lstBookingInfoList.Add(new ExcelExportModel());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = dr["RequestDate"].ToString(); //Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                            lstBookingInfoList[lstBookingInfoList.Count - 1].OrganizationName = dr["OrganizationName"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].OrganizationCode = dr["OrganizationCode"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpCode = dr["RequestedEmpId"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].PNRAmount = dr["PaymentAmount"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = dr["IsMandatoryTravel"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].IsVendorBooking = dr["IsVendorBooking"].ToString();
                        }
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                    dicList.Add("bookingList", lstBookingInfoList);
                }
            }
            catch (Exception ex)
            {
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(ex, "GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport", "CommonWebMethod/CommonWebMethods.cs");
                throw new Exception();
            }
            return dicList;
        }
        /// <summary>
        /// Get cxo dept mapping list
        /// </summary>
        /// <param name="cxoName"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<SodCXODeptMappingModels> GetcxoDeptMappingList(string cxoName, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstcxoDptMappingList = new List<SodCXODeptMappingModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_SodCXODeptMapping"
                };
                cmd.Parameters.Add("@cxoName", SqlDbType.VarChar).Value = cxoName;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstcxoDptMappingList.Add(new SodCXODeptMappingModels());
                        lstcxoDptMappingList[lstcxoDptMappingList.Count - 1].AllocatedDeptId = Convert.ToInt32(dr["DeptId"].ToString());
                        lstcxoDptMappingList[lstcxoDptMappingList.Count - 1].AllocatedDeptName = dr["DeptName"].ToString();
                        lstcxoDptMappingList[lstcxoDptMappingList.Count - 1].CXOName = dr["CXOName"].ToString();
                        lstcxoDptMappingList[lstcxoDptMappingList.Count - 1].IsActive = Convert.ToInt16(dr["MStatus"]);
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstcxoDptMappingList;
        }
        /// <summary>
        /// Validate Employee Code with DB
        /// </summary>
        /// <param name="empList"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static List<string> ValidateEmployeeCode(List<string> empList, int criteria)
        {
            StringBuilder sbelist = new StringBuilder();
            var empMatchList = new List<string>();

            foreach (var s in empList)
            {
                sbelist.Append(s.ToString().Trim() + ",");
            }
            var index = sbelist.ToString().LastIndexOf(',');
            if (index >= 0)
                sbelist.Remove(index, 1);

            var strConnString = ConnectionUtility.GetConnection();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodValidateEmpCode"
                };
                cmd.Parameters.Add("@empList", SqlDbType.VarChar, 2000).Value = sbelist.ToString().Trim();
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        empMatchList.Add(dr["empCode"].ToString().Trim());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return empMatchList;
        }
        /// <summary>
        /// Validate Employee Verticals
        /// </summary>
        /// <param name="empList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> ValidateEmployeeVerticals(List<string> empList, string verticalList, int criteria)
        {
            StringBuilder sbelist = new StringBuilder();
            var empMatchList = new List<string>();

            foreach (var s in empList)
            {
                sbelist.Append(s.ToString().Trim() + ",");
            }
            var index = sbelist.ToString().LastIndexOf(',');
            if (index >= 0)
                sbelist.Remove(index, 1);

            var strConnString = ConnectionUtility.GetConnection();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodValidateEmpVerticals"
                };
                cmd.Parameters.Add("@empList", SqlDbType.VarChar, 2000).Value = sbelist.ToString().Trim();
                cmd.Parameters.Add("@Verticals", SqlDbType.VarChar, 200).Value = verticalList.Trim();
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        empMatchList.Add(dr["empCode"].ToString().Trim());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return empMatchList;
        }
        /// <summary>
        /// Get Employee Code wise Details for Bulk Booking System
        /// </summary>
        /// <param name="empList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<EmployeeCodewiseDetailModel> GetEmployeeCodewiseDetails(List<string> empList, int criteria)
        {
            StringBuilder sbelist = new StringBuilder();
            var empDetailsList = new List<EmployeeCodewiseDetailModel>();

            foreach (var s in empList)
            {
                sbelist.Append(s.ToString().Trim() + ",");
            }
            var index = sbelist.ToString().LastIndexOf(',');
            if (index >= 0)
                sbelist.Remove(index, 1);

            var strConnString = ConnectionUtility.GetConnection();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetEmpCodewiseDetails"
                };
                cmd.Parameters.Add("@empList", SqlDbType.VarChar, 2000).Value = sbelist.ToString().Trim();
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        empDetailsList.Add(new EmployeeCodewiseDetailModel());
                        empDetailsList[empDetailsList.Count - 1].EmpCode = dr["empCode"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].FirstName = dr["FirstName"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].LastName = dr["LastName"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].Gender = dr["Gender"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].PhoneNo = dr["Phone"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].EmailId = dr["Email"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].Designation = dr["DesignationName"].ToString().Trim();
                        empDetailsList[empDetailsList.Count - 1].Department = dr["DepartmentName"].ToString().Trim();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return empDetailsList;
        }
        /// <summary>
        /// Get Sod Employee Booking History : Travel Desk  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeBookingHistoryList_TravelDesk(int? deptartmentId, int? empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var lstDept = new List<DepartmentModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingListEmpHistory_TravelDesk"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                    }

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            lstDept.Add(new DepartmentModels());
                            lstDept[lstDept.Count - 1].id = Convert.ToInt32(dr["department_id"].ToString());
                            lstDept[lstDept.Count - 1].dept_name = dr["dept_name"].ToString();
                        }
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("bookingList", lstBookingInfoList);
                dicList.Add("deptList", lstDept);
            }
            return dicList;
        }
        /// <summary>
        /// Get Sod Hotel Details : Travel Desk  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk(int? deptartmentId, int? empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var lstDept = new List<DepartmentModels>();
            var lstDeptOat = new List<DepartmentModels>();
            var lstoat = new List<OALTravelRequestMasterModel>();
            var dicList = new Dictionary<string, object>();
            var dicListOat = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodHotelList_TravelDesk"
                };
                cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = deptartmentId;
                cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (criteria == 1)
                    {
                        while (dr.Read())
                        {
                            lstBookingInfoList.Add(new TravelRequestModels());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                            lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                            lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].CancellationStatus = dr["usercancellation"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HodApprovalStatus = dr["HodApprovalStatus"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelName = dr["HotelName"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].clubId = Convert.ToInt32(dr["clubId"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].ApprovalDate = Convert.ToDateTime(dr["StatusDate"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Gender"].ToString();
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                lstDept.Add(new DepartmentModels());
                                lstDept[lstDept.Count - 1].id = Convert.ToInt32(dr["department_id"].ToString());
                                lstDept[lstDept.Count - 1].dept_name = dr["dept_name"].ToString();
                            }
                        }
                        dicList.Add("bookingList", lstBookingInfoList);
                        dicList.Add("deptList", lstDept);
                        return dicList;
                    }
                    else
                    {
                        while (dr.Read())
                        {
                            lstoat.Add(new OALTravelRequestMasterModel());
                            lstoat[lstoat.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                            lstoat[lstoat.Count - 1].RequestDate = Convert.ToDateTime(dr["RequestDate"].ToString());
                            lstoat[lstoat.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                            lstoat[lstoat.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                            lstoat[lstoat.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                            lstoat[lstoat.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                            lstoat[lstoat.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                            lstoat[lstoat.Count - 1].TravelDate = Convert.ToDateTime(dr["TravelDate"].ToString());
                            lstoat[lstoat.Count - 1].Pnr = dr["Pnr"].ToString();
                            lstoat[lstoat.Count - 1].FlightNo = dr["FlightNo"].ToString();
                            lstoat[lstoat.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                            lstoat[lstoat.Count - 1].EmailId = dr["EmailId"].ToString();
                            lstoat[lstoat.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                            lstoat[lstoat.Count - 1].CancellationStatus = dr["usercancellation"].ToString();
                            lstoat[lstoat.Count - 1].HodApprovalStatus = dr["HodApprovalStatus"].ToString();
                            lstoat[lstoat.Count - 1].HotelName = dr["HotelName"].ToString();
                            lstoat[lstoat.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                lstDeptOat.Add(new DepartmentModels());
                                lstDeptOat[lstDeptOat.Count - 1].id = Convert.ToInt32(dr["department_id"].ToString());
                                lstDeptOat[lstDeptOat.Count - 1].dept_name = dr["dept_name"].ToString();
                            }
                        }
                    }
                    dicListOat.Add("bookingoatList", lstoat);
                    dicListOat.Add("deptListOat", lstDeptOat);
                    return dicListOat;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        /// <summary>
        /// Get Sod Cab Booking Info
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> GetCabStatus(int reqId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingStatus = new List<string>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingCabStatus"
                };
                cmd.Parameters.Add("@ReqId", SqlDbType.Int).Value = reqId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingStatus.Add(dr["Comment"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingStatus;
        }
        /// <summary>
        /// Get Sod Hotel Booking Info
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> GetHotelStatus(int reqId, int criteria, int hId)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingStatus = new List<string>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodBookingHotelStatus"
                };
                cmd.Parameters.Add("@ReqId", SqlDbType.Int).Value = reqId;
                cmd.Parameters.Add("@hId", SqlDbType.Int).Value = reqId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingStatus.Add(dr["Comment"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBookingStatus;
        }

        /// <summary>
        /// Get Employee Booking History by Employee Code wise
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<TravelRequestModels> GetSodEmployeeBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept)
        {
            var strConnString = ConnectionUtility.GetConnection();
            DataSet ds = new DataSet();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var Isvbooking = IsVendorBooking.Equals(true) ? 1 : 0;

            string a = "SP_SodBookingListEmpHistory_ByEmployeeCode '" + empcode + "','" + Isvbooking + "','" + fdate + "','" + tdate + "','" + criteria + "','" + Dept + "'";
            SqlConnection con = new SqlConnection(strConnString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(a, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds); DataTable dt = new DataTable();
                var dr = cmd.ExecuteReader();
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        int i;
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            lstBookingInfoList.Add(new TravelRequestModels()
                            {
                                TravelRequestId = Convert.ToInt32(dt.Rows[i]["TravelRequestId"].ToString() != "" ? dt.Rows[i]["TravelRequestId"].ToString() : null),
                                BookingRequestDate = Convert.ToString(dt.Rows[i]["RequestDate"].ToString()),
                                TravelRequestCode = dt.Rows[i]["TravelRequestCode"].ToString() != "" ? dt.Rows[i]["TravelRequestCode"].ToString() : null,
                                TravelRequestTypeId = Convert.ToInt16(dt.Rows[i]["TravelRequestTypeId"].ToString() != "" ? dt.Rows[i]["TravelRequestTypeId"].ToString() : null),
                                RequestedEmpName = dt.Rows[i]["RequestedEmpName"].ToString() != "" ? dt.Rows[i]["RequestedEmpName"].ToString() : null,
                                RequestedEmpDesignation = dt.Rows[i]["RequestedEmpDesignation"].ToString() != "" ? dt.Rows[i]["RequestedEmpDesignation"].ToString() : null,
                                RequestedEmpDept = dt.Rows[i]["RequestedEmpDept"].ToString() != "" ? dt.Rows[i]["RequestedEmpDept"].ToString() : null,
                                ReasonForTravel = dt.Rows[i]["ReasonForTravel"].ToString() != "" ? dt.Rows[i]["ReasonForTravel"].ToString() : null,
                                TravelDate = dt.Rows[i]["TravelDate"].ToString() != "" ? dt.Rows[i]["TravelDate"].ToString() : null,
                                Sector = dt.Rows[i]["Sector"].ToString() != "" ? dt.Rows[i]["Sector"].ToString() : null,
                                SodBookingType = dt.Rows[i]["SodBookingTypeId"].ToString() != "" ? dt.Rows[i]["SodBookingTypeId"].ToString() : null,
                                BookingFor = dt.Rows[i]["BookingFor"].ToString() != "" ? dt.Rows[i]["BookingFor"].ToString() : null,
                                IsMandatoryTravel = Convert.ToInt16(dt.Rows[i]["IsMandatoryTravel"].ToString() != "" ? dt.Rows[i]["IsMandatoryTravel"].ToString() : null),
                                FlightNo = dt.Rows[i]["FlightNo"].ToString() != "" ? dt.Rows[i]["FlightNo"].ToString() : null,
                                Pnr = dt.Rows[i]["Pnr"].ToString() != "" ? dt.Rows[i]["Pnr"].ToString() : null,
                                BookingStatus = dt.Rows[i]["BookingStatus"].ToString() != "" ? dt.Rows[i]["BookingStatus"].ToString() : null,
                                HotelStatus = dt.Rows[i]["HotelStatus"].ToString() != "" ? dt.Rows[i]["HotelStatus"].ToString() : null,
                                CancellationStatus = dt.Rows[i]["usercancellation"].ToString() != "" ? dt.Rows[i]["usercancellation"].ToString() : null,
                                IsVendorBooking = Convert.ToBoolean(dt.Rows[i]["IsVendorBooking"].ToString() != "" ? dt.Rows[i]["IsVendorBooking"].ToString() : null),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            // }
            return lstBookingInfoList;
        }

        public static List<TravelRequestMasterModels> GetSodApproverBookingHistoryList_ByEmployeeCode(string empcode, int criteria, bool IsVendorBooking, string fdate, string tdate, string Dept)
        {
            var strConnString = ConnectionUtility.GetConnection();
            DataSet ds = new DataSet();
            var lstBookingInfoList = new List<TravelRequestMasterModels>();
            var Isvbooking = IsVendorBooking.Equals(true) ? 1 : 0;

            string a = "SP_SodBookingListEmpHistory_ByEmployeeCode '" + empcode + "','" + Isvbooking + "','" + fdate + "','" + tdate + "','" + criteria + "','" + Dept + "'";
            SqlConnection con = new SqlConnection(strConnString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(a, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds); DataTable dt = new DataTable();
                var dr = cmd.ExecuteReader();
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        int i;
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            lstBookingInfoList.Add(new TravelRequestMasterModels()
                            {
                                TravelRequestId = Convert.ToInt64(dt.Rows[i]["TravelRequestId"].ToString() != "" ? dt.Rows[i]["TravelRequestId"].ToString() : null),
                                RequestDate = Convert.ToDateTime(dt.Rows[i]["RequestDate"].ToString()),
                                TravelRequestCode = dt.Rows[i]["TravelRequestCode"].ToString() != "" ? dt.Rows[i]["TravelRequestCode"].ToString() : null,
                                RequestedEmpCode = dt.Rows[i]["RequestedEmpCode"].ToString() != "" ? dt.Rows[i]["RequestedEmpCode"].ToString() : null,
                                RequestedEmpName = dt.Rows[i]["RequestedEmpName"].ToString() != "" ? dt.Rows[i]["RequestedEmpName"].ToString() : null,
                                RequestedEmpDesignation = dt.Rows[i]["RequestedEmpDesignation"].ToString() != "" ? dt.Rows[i]["RequestedEmpDesignation"].ToString() : null,
                                RequestedEmpDept = dt.Rows[i]["RequestedEmpDept"].ToString() != "" ? dt.Rows[i]["RequestedEmpDept"].ToString() : null,
                                ReasonForTravel = dt.Rows[i]["ReasonForTravel"].ToString() != "" ? dt.Rows[i]["ReasonForTravel"].ToString() : null,
                                EmailId = dt.Rows[i]["EmailId"].ToString() != "" ? dt.Rows[i]["EmailId"].ToString() : null,
                                Phno = dt.Rows[i]["Phno"].ToString() != "" ? dt.Rows[i]["Phno"].ToString() : null,
                                ReasonForMandatoryTravel = dt.Rows[i]["ReasonForMandatoryTravel"].ToString() != "" ? dt.Rows[i]["ReasonForMandatoryTravel"].ToString() : null,
                                BookingFor = dt.Rows[i]["BookingFor"].ToString() != "" ? dt.Rows[i]["BookingFor"].ToString() : null,
                                IsMandatoryTravel = Convert.ToInt16(dt.Rows[i]["IsMandatoryTravel"].ToString() != "" ? dt.Rows[i]["IsMandatoryTravel"].ToString() : null),
                                //FlightNo = dt.Rows[i]["FlightNo"].ToString() != "" ? dt.Rows[i]["FlightNo"].ToString() : null,
                                Pnr = dt.Rows[i]["Pnr"].ToString() != "" ? dt.Rows[i]["Pnr"].ToString() : null,
                                BookingStatus = dt.Rows[i]["BookingStatus"].ToString() != "" ? dt.Rows[i]["BookingStatus"].ToString() : null,
                                //HotelStatus = dt.Rows[i]["HotelStatus"].ToString() != "" ? dt.Rows[i]["HotelStatus"].ToString() : null,
                                //CancellationStatus = dt.Rows[i]["usercancellation"].ToString() != "" ? dt.Rows[i]["usercancellation"].ToString() : null,
                                IsHotelRequired = Convert.ToBoolean(dt.Rows[i]["IsHotelRequired"].ToString() != "" ? dt.Rows[i]["IsHotelRequired"].ToString() : null),
                                //IsHotelRequired = Convert.ToBoolean(dt.Rows[i]["IsHotelRequired"].ToString() != "" ? dt.Rows[i]["IsHotelRequired"].ToString() : null),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            // }
            return lstBookingInfoList;
        }


        /// <summary>
        /// /// Get Bulk Booking Details : PNR wise
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<BulkUploadModels> GetBulkBookingPNRWiseDetails(DateTime fromdate, DateTime todate, string empcode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkBookingList"
                };
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromdate;
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = todate;
                cmd.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = empcode;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadModels());
                        lstBulkList[lstBulkList.Count - 1].TrnId = Convert.ToInt64(dr["TRId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FirstName = dr["Name"].ToString();
                        lstBulkList[lstBulkList.Count - 1].MobileNo = dr["MobileNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBulkList[lstBulkList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBulkList[lstBulkList.Count - 1].PNR = dr["PNR"].ToString();
                        lstBulkList[lstBulkList.Count - 1].PNR_Statuss = dr["PNR_Status"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Meal = dr["Meal"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Beverage = dr["Beverage"].ToString();
                        lstBulkList[lstBulkList.Count - 1].BookingType = dr["BookingType"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].IsHotelRequired = Convert.ToBoolean(dr["IsHotelRequired"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// /// Get Bulk Booking Details : PNR wise
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<BulkUpload_ExcelExportPNRWise> GetBulkBookingPNRWiseDetails_ExportToExcel(DateTime fromdate, DateTime todate, string empcode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUpload_ExcelExportPNRWise>();
            int counter = 0;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkBookingList"
                };
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromdate;
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = todate;
                cmd.Parameters.Add("@EmpCode", SqlDbType.VarChar).Value = empcode;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        counter = counter + 1;
                        lstBulkList.Add(new BulkUpload_ExcelExportPNRWise());
                        lstBulkList[lstBulkList.Count - 1].SrNo = counter;
                        lstBulkList[lstBulkList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Name = dr["Name"].ToString();
                        lstBulkList[lstBulkList.Count - 1].MobileNo = dr["MobileNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBulkList[lstBulkList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBulkList[lstBulkList.Count - 1].PNR = dr["PNR"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Meal = dr["Meal"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Beverage = dr["Beverage"].ToString();
                        lstBulkList[lstBulkList.Count - 1].IsHotelRequired = Convert.ToBoolean(dr["IsHotelRequired"].ToString());
                        lstBulkList[lstBulkList.Count - 1].BookingType = dr["BookingType"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TrnId = dr["TRId"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// Get Sod Bulk Booking Agency Code Rights
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> GetEmployeeBookingAgencyRight(string empCode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstAgencyCode = new List<string>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetEmpAgencyRightInfo"
                };
                cmd.Parameters.Add("@empCode", SqlDbType.VarChar).Value = empCode;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstAgencyCode.Add(dr["AgencyCode"].ToString().Trim());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstAgencyCode;
        }
        /// <summary>
        /// /// Get Bulk Booking Master List :For HOD Booking
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<BulkUploadMasterModels> GetBulkBookingHODApprovalList_MasterData(string empcode, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadMasterModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkBookingApprovalList"
                };
                cmd.Parameters.Add("@EmpId", SqlDbType.VarChar).Value = empcode;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadMasterModels());
                        lstBulkList[lstBulkList.Count - 1].TRId = Convert.ToInt64(dr["TRId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TransactionDate = Convert.ToDateTime(dr["TransactionDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].FileName = dr["FileName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedById = dr["CreatedById"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedByName = dr["CreatedByName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FileStatus = dr["FileStatus"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// Get hotel bulk booking data for traveldesk
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<BulkUploadMasterModels> GetBulkBookingHotelListData(string bookingType)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadMasterModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkBookingHotelList"
                };
                cmd.Parameters.Add("@bookingType", SqlDbType.VarChar).Value = bookingType;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadMasterModels());
                        lstBulkList[lstBulkList.Count - 1].TRId = Convert.ToInt64(dr["TRId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TransactionDate = Convert.ToDateTime(dr["TransactionDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].FileName = dr["FileName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedById = dr["CreatedById"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedByName = dr["CreatedByName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FileStatus = dr["FileStatus"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelStatus = Convert.ToInt16(dr["HotelStatus"].ToString());
                        lstBulkList[lstBulkList.Count - 1].BookingType = dr["BookingType"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// Get Oat Data for helpdesk panel
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetOatData(string travelId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmployeeList = new List<OALTravelRequestMasterModel>();

            var dicList = new Dictionary<string, object>();
            var lstOatList = new List<OALTravelRequestMasterModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_OatDetailInfo"
                };
                cmd.Parameters.Add("@travelId", SqlDbType.VarChar).Value = travelId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 1;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new OALTravelRequestMasterModel());
                        //string date_of_birth = dr["DOB"].ToString().Remove(10);
                        //string dob = date_of_birth.Substring(8, 2) + "/" + date_of_birth.Substring(5, 2) + "/" + date_of_birth.Substring(0, 4);

                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestDate = DateTime.Parse(dr["RequestDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpCode = dr["RequestedEmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Gender = dr["Gender"].ToString(); ;
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Phno = dr["Phno"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Passengers = Convert.ToInt16(dr["Passengers"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].Meals = dr["Meals"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].PnrAmount = Convert.ToDecimal(dr["PnrAmount"].ToString());
                        if (dr["BookingStatus"].ToString() == "Approved")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + "by HOD";
                        }
                        else if (dr["BookingStatus"].ToString() == "Close")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " ✔";
                        }
                        else
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        }
                        lstEmployeeList[lstEmployeeList.Count - 1].StatusDate = Convert.ToDateTime(dr["StatusDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].address = dr["address"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].city = dr["city"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].IsCabRequired = Convert.ToBoolean(dr["IsCabRequired"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].IsHotelRequired = Convert.ToBoolean(dr["IsHotelRequired"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].OriginPlace = dr["OriginPlace"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].DestinationPlace = dr["DestinationPlace"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].FlightInfo = dr["FlightInfo"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].TravelDate = Convert.ToDateTime(dr["TravelDate"].ToString());

                    }
                    dicList.Add("oatlist", lstEmployeeList);

                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

            return dicList;

        }

        /// <summary>
        /// Get approved oat requests for traveldesk panel
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetApprovedRequests(string empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmpDetails = new List<OALTravelRequestMasterModel>();

            var dicList = new Dictionary<string, object>();
            var lstEmployeeList = new List<OALTravelRequestMasterModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_OatDetailInfo"
                };
                cmd.Parameters.Add("@empFilter", SqlDbType.VarChar).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 2;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new OALTravelRequestMasterModel());
                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestDate = Convert.ToDateTime(dr["RequestDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpCode = dr["RequestedEmpCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        if (dr["BookingStatus"].ToString() == "Approved")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " by HOD";
                        }
                        else if (dr["BookingStatus"].ToString() == "Close")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " ✔";
                        }
                        else
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        }
                        lstEmployeeList[lstEmployeeList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].StatusDate = Convert.ToDateTime(dr["StatusDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].ITHResponseStatus = dr["ITHResponseStatus"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].FinancialApproval = dr["FinancialApproval"].ToString();

                    }
                    dicList.Add("approvedList", lstEmployeeList);
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return dicList;
        }
        /// <summary>
        /// Get details for the user oat travel history
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetOatHistoryRequests(string empId, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstEmpDetails = new List<OALTravelRequestMasterModel>();

            var dicList = new Dictionary<string, object>();
            var lstEmployeeList = new List<OALTravelRequestMasterModel>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_OatDetailInfo"
                };
                cmd.Parameters.Add("@empFilter", SqlDbType.VarChar).Value = empId;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = 3;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstEmployeeList.Add(new OALTravelRequestMasterModel());

                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestDate = Convert.ToDateTime(dr["RequestDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        if (dr["BookingStatus"].ToString() == "Approved")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " by HOD";
                        }
                        else if (dr["BookingStatus"].ToString() == "Close")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " ✔";
                        }
                        else if (dr["BookingStatus"].ToString() == "Rejected by TravelDesk" || dr["BookingStatus"].ToString() == "Rejected by HOD")
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString() + " ✘";
                        }
                        else
                        {
                            lstEmployeeList[lstEmployeeList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        }
                        lstEmployeeList[lstEmployeeList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].StatusDate = Convert.ToDateTime(dr["StatusDate"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].Passengers = Convert.ToInt16(dr["Passengers"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].IsCabRequired = Convert.ToBoolean(dr["IsCabRequired"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].IsHotelRequired = Convert.ToBoolean(dr["IsHotelRequired"].ToString());
                        lstEmployeeList[lstEmployeeList.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                        lstEmployeeList[lstEmployeeList.Count - 1].CancellationStatus = dr["usercancellation"].ToString();

                    }
                    dicList.Add("tList", lstEmployeeList);


                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

            return dicList;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptartmentId"></param>
        /// <param name="empId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetHotelandUserInfo(string rqid1, string rqid2, string hid1, string hid2, int criteria, string sodOat)
        {
            //StringBuilder sbelist = new StringBuilder();
            //foreach (var s in requestList)
            //{
            //    sbelist.Append(s.ToString().Trim() + ",");
            //}
            //var index = sbelist.ToString().LastIndexOf(',');
            //if (index >= 0)
            //    sbelist.Remove(index, 1);

            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodPassengerInfoForHotelRequest"
                };
                cmd.Parameters.Add("@trid1", SqlDbType.BigInt).Value = Convert.ToInt64(rqid1.ToString().Trim());
                cmd.Parameters.Add("@hotelid1", SqlDbType.Int).Value = Convert.ToInt32(hid1.ToString().Trim());
                if (rqid2.Length > 0)
                {
                    cmd.Parameters.Add("@trid2", SqlDbType.BigInt).Value = Convert.ToInt64(rqid2.ToString().Trim());
                    cmd.Parameters.Add("@hotelid2", SqlDbType.Int).Value = Convert.ToInt32(hid2.ToString().Trim());
                }
                else
                {
                    cmd.Parameters.Add("@trid2", SqlDbType.BigInt).Value = 0;
                    cmd.Parameters.Add("@hotelid2", SqlDbType.Int).Value = 0;
                }
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinDate = Convert.ToDateTime(dr["CheckInDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckoutDate = Convert.ToDateTime(dr["CheckOutDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].accomodationRequired = Convert.ToBoolean(dr["AirportTransport"].ToString());
                        var flight = dr["FlightNo"].ToString();
                        var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = flightNo;
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Phone = dr["Phno"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Title"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinTime = dr["CheckinTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelType = dr["HotelType"].ToString();
                        if (criteria == 1)
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].DepartureTime = dr["DepartureTime"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].ArrivalTime = dr["ArrivalTime"].ToString();
                        }
                        if (dr["HotelType"].ToString() == "Non-Contractual")
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelPrice = Convert.ToDecimal(dr["HotelPrice"].ToString());
                        }
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelCity = dr["City"].ToString();
                        if (criteria == 1)
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].IsCabRequiredAsPerETA = Convert.ToBoolean(dr["IsCabRequiredAsPerETA"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].CabPickupTime = dr["CabPickupTime"].ToString();
                        }
                        lstBookingInfoList[lstBookingInfoList.Count - 1].PassEmailId = dr["EmailId"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpCode = dr["RequestedEmpCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelCurrencyCode = dr["HotelCurrencyCode"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("PassengerInfoForHotelRequest", lstBookingInfoList);
            }
            return dicList;
        }
        public static List<BulkUploadModels> GetHotelListPopup(string TrId)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkBookingListHotelPopup"
                };
                cmd.Parameters.Add("@travelRqstId", SqlDbType.Int).Value = TrId;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadModels());
                        lstBulkList[lstBulkList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FirstName = dr["FirstName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Title = dr["Title"].ToString();
                        lstBulkList[lstBulkList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBulkList[lstBulkList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBulkList[lstBulkList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBulkList[lstBulkList.Count - 1].PNR = dr["PNR"].ToString();
                        lstBulkList[lstBulkList.Count - 1].PNR_Statuss = dr["PNR_Status"].ToString();
                        lstBulkList[lstBulkList.Count - 1].BookingType = dr["BookingType"].ToString();
                        lstBulkList[lstBulkList.Count - 1].LastName = dr["LastName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelCity = dr["HotelCity"].ToString();
                        lstBulkList[lstBulkList.Count - 1].IsHotelRequired = Convert.ToBoolean(dr["IsHotelRequired"].ToString());
                        lstBulkList[lstBulkList.Count - 1].BookingType = dr["BookingType"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CheckInDate = Convert.ToDateTime(dr["CheckInDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].CheckOutDate = Convert.ToDateTime(dr["CheckOutDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].CheckinTime = dr["CheckinTime"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CheckoutTime = dr["CheckoutTime"].ToString();
                        lstBulkList[lstBulkList.Count - 1].AirportTransport = Convert.ToBoolean(dr["AirportTransport"].ToString());
                        lstBulkList[lstBulkList.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelName = dr["HotelName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                        lstBulkList[lstBulkList.Count - 1].IsAllocated = Convert.ToInt16(dr["HotelAllocated"].ToString());
                        lstBulkList[lstBulkList.Count - 1].sharingId = Convert.ToInt16(dr["shareId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].Designation = dr["Designation"].ToString();
                        lstBulkList[lstBulkList.Count - 1].clubId = Convert.ToInt32(dr["clubId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].PrimaryEmail = dr["PrimaryEmail"].ToString();
                        lstBulkList[lstBulkList.Count - 1].SecondaryEmail = dr["SecondaryEmail"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelType = dr["HotelType"].ToString();
                        lstBulkList[lstBulkList.Count - 1].BReqId = Convert.ToInt64(dr["BReqId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].IsBookingcancelled = Convert.ToBoolean(dr["IsBookingcancelled"]);
                        lstBulkList[lstBulkList.Count - 1].HodApprovalStatus = dr["HodApprovalStatus"].ToString();

                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// get bulk user info for hotel
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="criteria"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetBulkHotelandUserInfo(Int64 travelRequestID, Int32 clubid, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_BulkPassengerInfoForHotelRequest"
                };
                cmd.Parameters.Add("@travelRqstId", SqlDbType.Int).Value = travelRequestID;
                cmd.Parameters.Add("@clubid", SqlDbType.Int).Value = clubid;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestID"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpCode = dr["EmployeeCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["Designation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["Department"].ToString();
                        //lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinDate = Convert.ToDateTime(dr["CheckInDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckoutDate = Convert.ToDateTime(dr["CheckOutDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].accomodationRequired = Convert.ToBoolean(dr["AirportTransport"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Phone = dr["MobileNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Title"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinTime = dr["CheckinTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelType = dr["HotelType"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].sharingId = Convert.ToInt16(dr["sharingId"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("InfoForHotelRequest", lstBookingInfoList);
            }
            return dicList;
        }
        /// <summary>
        /// save hod response
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<ITHResponseDetailModels> saveHODResponseData(string IdList, int criteria)
        {
            var lstBookingInfoList = new List<ITHResponseDetailModels>();
            var strConnString = ConnectionUtility.GetConnection();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_OatITHResponse_byHOD"
                };
                cmd.Parameters.Add("@IdList", SqlDbType.VarChar, 2000).Value = IdList.Trim();
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new ITHResponseDetailModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ResponseDate = Convert.ToDateTime(dr["ResponseDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].OriginPlace = dr["OriginPlace"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].DestinationPlace = dr["DestinationPlace"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = Convert.ToDateTime(dr["TravelDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightName = dr["FlightName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].DepartureTime = dr["DepartureTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ArrivalTime = dr["ArrivalTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightType = dr["FlightType"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                return lstBookingInfoList;
            }
        }
        /// <summary>
        /// get requests not approved by hod- exceptional cases
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetHotelInfoExceptional(string trid, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodHotelList_TravelDesk_Exceptional"
                };
                cmd.Parameters.Add("@trid", SqlDbType.VarChar).Value = trid;
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CancellationStatus = dr["usercancellation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HodApprovalStatus = dr["HodApprovalStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelName = dr["HotelName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].clubId = Convert.ToInt32(dr["clubId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Gender"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("hotelExList", lstBookingInfoList);
            }
            return dicList;
        }
        /// <summary>
        /// Get PriceList for Sod Hotel booking
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<HotelRequestApprovalModel> GetHotelBillingDetails(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstbList = new List<HotelRequestApprovalModel>();

            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SODGetBillingInfo"
                };
                cmd.Parameters.Add("@fromdate", SqlDbType.VarChar, 10).Value = fromdate;
                cmd.Parameters.Add("@todate", SqlDbType.VarChar, 10).Value = todate;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type; //contractual/non-contractual
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstbList.Add(new HotelRequestApprovalModel());
                        lstbList[lstbList.Count - 1].Id = Convert.ToInt64(dr["SNo"].ToString());
                        lstbList[lstbList.Count - 1].TravelRequestId = Convert.ToInt64(dr["TravelRequestId"].ToString());
                        lstbList[lstbList.Count - 1].HotelName = dr["HotelName"].ToString();
                        lstbList[lstbList.Count - 1].ApprovalDate = Convert.ToDateTime(dr["ApprovalDate"]);
                        lstbList[lstbList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                        lstbList[lstbList.Count - 1].HotelCode = dr["HotelCode"].ToString();
                        lstbList[lstbList.Count - 1].HotelPrice = Convert.ToDecimal(dr["HotelPrice"].ToString());
                        lstbList[lstbList.Count - 1].HotelType = dr["HotelType"].ToString();
                        lstbList[lstbList.Count - 1].HotelAddress = dr["HotelAddress"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstbList;
        }
        /// <summary>
        /// Get PriceList for Sod Hotel booking for the MIS purpose
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<ExcelExportModelBilling> GetHotelBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trid)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstbList = new List<ExcelExportModelBilling>();

            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SODGetBillingInfo"
                };
                cmd.Parameters.Add("@fromdate", SqlDbType.VarChar, 10).Value = fromdate;
                cmd.Parameters.Add("@todate", SqlDbType.VarChar, 10).Value = todate;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Parameters.Add("@TrId", SqlDbType.BigInt).Value = trid;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstbList.Add(new ExcelExportModelBilling());
                        lstbList[lstbList.Count - 1].SNo = Convert.ToInt32(dr["SNo"].ToString());
                        lstbList[lstbList.Count - 1].TravelRequestId = dr["TravelRequestId"].ToString();
                        lstbList[lstbList.Count - 1].HOTAC = dr["HOTAC"].ToString();
                        lstbList[lstbList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstbList[lstbList.Count - 1].StaffId = dr["RequestedEmpCode"].ToString();
                        lstbList[lstbList.Count - 1].ETA = dr["ETA"].ToString();
                        lstbList[lstbList.Count - 1].City_Name = dr["City"].ToString();
                        lstbList[lstbList.Count - 1].Check_In = dr["CheckinDate"].ToString();
                        lstbList[lstbList.Count - 1].Check_Out = dr["CheckOutDate"].ToString();
                        lstbList[lstbList.Count - 1].Check_In_Time = dr["CheckinTime"].ToString();
                        lstbList[lstbList.Count - 1].Check_Out_Time = dr["CheckoutTime"].ToString();
                        lstbList[lstbList.Count - 1].Gender = dr["Gender"].ToString();
                        lstbList[lstbList.Count - 1].Name = dr["RequestedEmpName"].ToString();
                        lstbList[lstbList.Count - 1].Department = dr["RequestedEmpDept"].ToString();
                        lstbList[lstbList.Count - 1].Room_Night = dr["Room_Night"].ToString();
                        lstbList[lstbList.Count - 1].Room_Type = dr["Room_Type"].ToString();
                        lstbList[lstbList.Count - 1].HOD_Approval = dr["Status"].ToString();
                        lstbList[lstbList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                        lstbList[lstbList.Count - 1].ApproverName1 = dr["ApproverNameLevel1"].ToString();
                        lstbList[lstbList.Count - 1].ApproverName2 = dr["ApproverNameLevel2"].ToString();
                        lstbList[lstbList.Count - 1].AmountPerNight = dr["AmountPerNight"].ToString();
                        lstbList[lstbList.Count - 1].TotalAmount = dr["TotalAmount"].ToString();
                        lstbList[lstbList.Count - 1].Taxes_IsInclusive = dr["TaxesIncluded"].ToString();
                        lstbList[lstbList.Count - 1].ApprovalStatus = dr["Status"].ToString();
                        lstbList[lstbList.Count - 1].HotelType = dr["HotelType"].ToString();
                        lstbList[lstbList.Count - 1].CurrencyCode = dr["HotelCurrencyCode"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstbList;
        }
        /// <summary>
        /// Get Hotel Bulk Booking Billing Details Master
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<BulkUploadMasterModels> GetHotelBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadMasterModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SODGetBillingInfo"
                };
                cmd.Parameters.Add("@fromdate", SqlDbType.VarChar, 10).Value = fromdate;
                cmd.Parameters.Add("@todate", SqlDbType.VarChar, 10).Value = todate;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type; //contractual/non-contractual
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;

                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadMasterModels());
                        lstBulkList[lstBulkList.Count - 1].SNo = Convert.ToInt32(dr["SNo"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TRId = Convert.ToInt64(dr["TRId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TransactionDate = Convert.ToDateTime(dr["TransactionDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].FileName = dr["FileName"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedById = dr["CreatedById"].ToString();
                        lstBulkList[lstBulkList.Count - 1].CreatedByName = dr["CreatedByName"].ToString();
                        //lstBulkList[lstBulkList.Count - 1].FileStatus = dr["FileStatus"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HotelStatus = Convert.ToInt16(dr["HotelStatus"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
        /// <summary>
        /// Get Sod Hotel Details : Travel Desk  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetSodEmployeeHotelList_TravelDesk_DoubleOccupancy(string newTrid, string existingTrid, string hotelrqstid, string exishotelid)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var lstDept = new List<DepartmentModels>();
            var lstDeptOat = new List<DepartmentModels>();
            var lstoat = new List<OALTravelRequestMasterModel>();
            var dicList = new Dictionary<string, object>();
            var dicListOat = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodHotelList_TravelDesk_DoubleOccupancy"
                };
                cmd.Parameters.Add("@newTrid", SqlDbType.BigInt).Value = Convert.ToInt64(newTrid);
                cmd.Parameters.Add("@existingTrid", SqlDbType.BigInt).Value = Convert.ToInt64(existingTrid);
                cmd.Parameters.Add("@hotelid1", SqlDbType.Int).Value = Convert.ToInt32(hotelrqstid);
                cmd.Parameters.Add("@hotelid2", SqlDbType.Int).Value = Convert.ToInt32(exishotelid);
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingStatus = dr["BookingStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelStatus = dr["HotelStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CancellationStatus = dr["usercancellation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HodApprovalStatus = dr["HodApprovalStatus"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelName = dr["HotelName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].clubId = Convert.ToInt32(dr["clubId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ApprovalDate = Convert.ToDateTime(dr["StatusDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Gender"].ToString();
                    }

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            lstDept.Add(new DepartmentModels());
                            lstDept[lstDept.Count - 1].id = Convert.ToInt32(dr["department_id"].ToString());
                            lstDept[lstDept.Count - 1].dept_name = dr["dept_name"].ToString();
                        }
                    }
                    dicList.Add("bookingList", lstBookingInfoList);
                    dicList.Add("deptList", lstDept);
                    return dicList;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        /// <summary>
        /// Find Similar Hotel Allocation Data
        /// </summary>
        /// <param name="trId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<string> GetHotelApprovalStatus(Int64 trId, Int16 criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstApprovalStatus = new List<string>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetHotelApprovalStatus"
                };
                cmd.Parameters.Add("@TrId", SqlDbType.BigInt).Value = trId;
                cmd.Parameters.Add("@criteria", SqlDbType.VarChar).Value = criteria;
                cmd.Connection = con;

                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstApprovalStatus.Add(dr["ApproverStatus"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }

            }
            return lstApprovalStatus;
        }
        /// <summary>
        /// Find Similar Hotel Allocation Data
        /// </summary>
        /// <param name="hotelname"></param>
        /// <param name="checkin"></param>
        /// <param name="hotelcity"></param>
        /// <returns></returns>
        public static List<TravelRequestHotelDetailModels> FindSimilarHotelAllocationData(Int64 TravelRequestId, string hotelname, DateTime checkin, string hotelcity)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lsthotelList = new List<TravelRequestHotelDetailModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_HotelSimilarAllocation"
                };
                cmd.Parameters.Add("@TravelRequestId", SqlDbType.BigInt).Value = TravelRequestId;
                cmd.Parameters.Add("@hotelname", SqlDbType.VarChar).Value = hotelname;
                cmd.Parameters.Add("@checkin", SqlDbType.DateTime).Value = checkin;
                cmd.Parameters.Add("@hotelcity", SqlDbType.VarChar).Value = hotelcity; //contractual/non-contractual
                cmd.Connection = con;

                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lsthotelList.Add(new TravelRequestHotelDetailModels());
                        lsthotelList[lsthotelList.Count - 1].clubId = Convert.ToInt32(dr["clubId"].ToString());
                        lsthotelList[lsthotelList.Count - 1].TravelRequestId = Convert.ToInt64(dr["TravelRequestId"].ToString());
                        lsthotelList[lsthotelList.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                        lsthotelList[lsthotelList.Count - 1].Designation = dr["DesignationName"].ToString();
                        lsthotelList[lsthotelList.Count - 1].EmployeeName = dr["FirstName"].ToString();
                        lsthotelList[lsthotelList.Count - 1].EmployeeCode = dr["EmployeeCode"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lsthotelList;
        }
        /// <summary>
        /// Get Hotel and User Info For Responsive Mail
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="criteria"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetHotelandUserInfoForResponsiveMail(string rqid1, string rqid2, string hid1, string hid2, int criteria, string sodOat)
        {
            //StringBuilder sbelist = new StringBuilder();
            //foreach (var s in requestList)
            //{
            //    sbelist.Append(s.ToString().Trim() + ",");
            //}
            //var index = sbelist.ToString().LastIndexOf(',');
            //if (index >= 0)
            //    sbelist.Remove(index, 1);

            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var dicList = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodPassengerInfoForHotelRequestResponsive"
                };
                cmd.Parameters.Add("@trid1", SqlDbType.BigInt).Value = Convert.ToInt64(rqid1.ToString().Trim());
                cmd.Parameters.Add("@hotelid1", SqlDbType.Int).Value = Convert.ToInt32(hid1.ToString().Trim());
                if (rqid2.Length > 0)
                {
                    cmd.Parameters.Add("@trid2", SqlDbType.BigInt).Value = Convert.ToInt64(rqid2.ToString().Trim());
                    cmd.Parameters.Add("@hotelid2", SqlDbType.Int).Value = Convert.ToInt32(hid2.ToString().Trim());
                }
                else
                {
                    cmd.Parameters.Add("@trid2", SqlDbType.BigInt).Value = 0;
                    cmd.Parameters.Add("@hotelid2", SqlDbType.Int).Value = 0;
                }
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpId = Convert.ToInt32(dr["RequestedEmpId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinDate = Convert.ToDateTime(dr["CheckInDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckoutDate = Convert.ToDateTime(dr["CheckOutDate"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].accomodationRequired = Convert.ToBoolean(dr["AirportTransport"].ToString());
                        var flight = dr["FlightNo"].ToString();
                        var flightNo = flight.Substring(flight.LastIndexOf(',') + 1).Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = flightNo;
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Phone = dr["Phno"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Title = dr["Title"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinTime = dr["CheckinTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelType = dr["HotelType"].ToString();
                        if (criteria == 1)
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].DepartureTime = dr["DepartureTime"].ToString();
                            lstBookingInfoList[lstBookingInfoList.Count - 1].ArrivalTime = dr["ArrivalTime"].ToString();
                        }
                        if (dr["HotelType"].ToString() == "Non-Contractual")
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].HotelPrice = Convert.ToDecimal(dr["HotelPrice"].ToString());
                        }
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelCity = dr["City"].ToString();
                        if (criteria == 1)
                        {
                            lstBookingInfoList[lstBookingInfoList.Count - 1].IsCabRequiredAsPerETA = Convert.ToBoolean(dr["IsCabRequiredAsPerETA"].ToString());
                            lstBookingInfoList[lstBookingInfoList.Count - 1].CabPickupTime = dr["CabPickupTime"].ToString();
                        }
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelConfirmationNo = dr["HotelConfirmationNo"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
                dicList.Add("PassengerInfoForHotelRequestResponsive", lstBookingInfoList);
            }
            return dicList;
        }
        /// <summary>
        /// get requests with for checkin-checkout time confirmation by user & hotel
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GetHotelDetailCheckTimeConflict()
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBookingInfoList = new List<TravelRequestModels>();
            var lstDept = new List<DepartmentModels>();
            var lstDeptOat = new List<DepartmentModels>();
            var lstoat = new List<OALTravelRequestMasterModel>();
            var dicList = new Dictionary<string, object>();
            var dicListOat = new Dictionary<string, object>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_Sod_GetTravelDesk_HotelDetailCheckTimeConflict"
                };
                //cmd.Parameters.Add("@newTrid", SqlDbType.BigInt).Value = Convert.ToInt64(newTrid);
                //cmd.Parameters.Add("@existingTrid", SqlDbType.BigInt).Value = Convert.ToInt64(existingTrid); ;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBookingInfoList.Add(new TravelRequestModels());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingRequestDate = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy HH:mm");
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelRequestCode = dr["TravelRequestCode"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDesignation = dr["RequestedEmpDesignation"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].RequestedEmpDept = dr["RequestedEmpDept"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].ReasonForTravel = dr["ReasonForTravel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].TravelDate = dr["TravelDate"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].SodBookingType = dr["SodBookingTypeId"].ToString().Trim();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].BookingFor = dr["BookingFor"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].IsMandatoryTravel = Convert.ToInt16(dr["IsMandatoryTravel"]);
                        lstBookingInfoList[lstBookingInfoList.Count - 1].FlightNo = dr["FlightNo"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].Pnr = dr["Pnr"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].EmailId = dr["EmailId"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].HotelName = dr["HotelName"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinTime = dr["CheckinTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckoutTime = dr["CheckoutTime"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckinTimeHotel = dr["CheckinTimeHotel"].ToString();
                        lstBookingInfoList[lstBookingInfoList.Count - 1].CheckoutTimeHotel = dr["CheckoutTimeHotel"].ToString();
                    }
                    dicList.Add("bookingTimeList", lstBookingInfoList);
                    return dicList;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        /// <summary>
        /// Get SMS data
        /// </summary>
        /// <param name="currentDate"></param>
        /// <param name="time1"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static List<TravelRequestModels> GetSMSData(DateTime currentDate, string time1, string time)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var List = new List<TravelRequestModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Sp_GetSMSUserData"
                };
                cmd.Parameters.Add("@arrivalDate", SqlDbType.DateTime).Value = currentDate;
                cmd.Parameters.Add("@arrivalTime1", SqlDbType.VarChar).Value = time1;
                cmd.Parameters.Add("@arrivalTime2", SqlDbType.VarChar).Value = time;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        List.Add(new TravelRequestModels());
                        List[List.Count - 1].RequestedEmpName = dr["RequestedEmpName"].ToString();
                        List[List.Count - 1].Phone = dr["Phno"].ToString();
                        List[List.Count - 1].ArrivalTime = dr["ArrivalTime"].ToString();
                        List[List.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        List[List.Count - 1].Id = Convert.ToInt32(dr["Id"].ToString());
                        List[List.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return List;
        }

        /// <summary>
        /// Get Sod Employee Booking History  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<string> getListForMailToTrigger()
        {
            var strConnString = ConnectionUtility.GetConnection();
            var TravelReqiestIdList = new List<String>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SodBookingListForMailToTrigger"
                };
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {

                        TravelReqiestIdList.Add(dr["TravelRequestId"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return TravelReqiestIdList;
        }


        /// <summary>
        /// Get Sod Employee Booking History  
        /// </summary>
        /// <param name="designationId"></param>
        /// <param name="deptartmentId"></param>
        ///  /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<TravelRequestHotelDetailModels> GetListForReminderMailToTrigger(int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var TravelReqiestIdList = new List<String>();
            var List = new List<TravelRequestHotelDetailModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_TravelDeskAutoReminderJob_PendingApproval"
                };
                cmd.Parameters.Add("@Criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (criteria == 1)
                    {
                        while (dr.Read())
                        {
                            List.Add(new TravelRequestHotelDetailModels());
                            List[List.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                        }
                    }
                    if (criteria == 2)
                    {
                        while (dr.Read())
                        {
                            List.Add(new TravelRequestHotelDetailModels());
                            List[List.Count - 1].TravelRequestId = Convert.ToInt32(dr["TravelRequestId"].ToString());
                            List[List.Count - 1].HotelRequestId = Convert.ToInt32(dr["HotelRequestId"].ToString());
                        }
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return List;
        }

        /// <summary>
        /// Geet HOD Name, EMail and EmpID
        /// </summary>
        /// <param name="verticalId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static string GetHODByTravelRequestId(String TravlelRequestID, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var details = string.Empty;
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodGetHODByTravelRequestId"
                };
                cmd.Parameters.Add("@TravelRequestId", SqlDbType.VarChar).Value = TravlelRequestID;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    details = cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return details;
        }
        /// <summary>
        /// Geet HOD Name, EMail and EmpID
        /// </summary>
        /// <param name="verticalId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static List<viewOatDetailsModal> GetOATBookingList(Int64 oatReqID, int criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var List = new List<viewOatDetailsModal>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_GetOATBookingList"
                };
                cmd.Parameters.Add("@OATTravelRequestId", SqlDbType.VarChar).Value = oatReqID;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        List.Add(new viewOatDetailsModal());
                        List[List.Count - 1].EmployeeCode = dr["EmployeeCode"].ToString();
                        List[List.Count - 1].FirstName = dr["FirstName"].ToString();
                        List[List.Count - 1].LastName = dr["LastName"].ToString();
                        List[List.Count - 1].OriginPlace = dr["OriginPlace"] is DBNull ? "" : dr["OriginPlace"].ToString();
                        List[List.Count - 1].DestinationPlace = dr["DestinationPlace"] is DBNull ? "" : dr["DestinationPlace"].ToString();
                        List[List.Count - 1].DepartureDate = dr["DepartureDate"] is DBNull ? Convert.ToDateTime("01/01/1990") : Convert.ToDateTime(dr["DepartureDate"]);
                        List[List.Count - 1].DepartureTime = dr["DepartureTime"] is DBNull ? "" : dr["DepartureTime"].ToString();
                        List[List.Count - 1].ID = dr["ID"] is DBNull ? 0 : Convert.ToInt64(dr["ID"]);
                        List[List.Count - 1].FlightType = dr["FlightType"] is DBNull ? "" : dr["FlightType"].ToString();
                        List[List.Count - 1].IsFlightRequired = dr["IsFlightRequired"].ToString();
                        List[List.Count - 1].IsHotelRequired = dr["IsHotelRequired"].ToString();
                        List[List.Count - 1].City = dr["HotelCity"].ToString();
                        List[List.Count - 1].CheckInDate = dr["CheckInDate"] is DBNull ? Convert.ToDateTime("01/01/1990") : Convert.ToDateTime(dr["CheckInDate"]);
                        List[List.Count - 1].CheckOutDate = dr["CheckOutDate"] is DBNull ? Convert.ToDateTime("01/01/1990") : Convert.ToDateTime(dr["CheckOutDate"]);
                        List[List.Count - 1].IsCancelled = dr["IsBookingcancelled"] is DBNull ? false : Convert.ToBoolean(dr["IsBookingcancelled"].ToString());
                        List[List.Count - 1].breqId = dr["BReqId"] is DBNull ? 0 : Convert.ToInt64(dr["BReqId"]);
                        List[List.Count - 1].OATTravelRequestId = Convert.ToInt64(dr["OATTravelRequestId"]);
                        List[List.Count - 1].TrnId = Convert.ToInt64(dr["TravelRequestId"]);
                        List[List.Count - 1].PassengerID = Convert.ToInt64(dr["PassengerID"]);
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return List;
        }

        public static IList<ExcelExportOATModelBilling> GetFlightBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trid)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstbList = new List<ExcelExportOATModelBilling>();

            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_SodOatBillingInfo"
                };
                cmd.Parameters.Add("@fromdate", SqlDbType.VarChar, 10).Value = fromdate;
                cmd.Parameters.Add("@todate", SqlDbType.VarChar, 10).Value = todate;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Parameters.Add("@TrId", SqlDbType.BigInt).Value = trid;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    
                    while (dr.Read())
                    {
                       
                        lstbList.Add(new ExcelExportOATModelBilling());
                        lstbList[lstbList.Count - 1].Sno = Convert.ToInt32(dr["SNo"].ToString());
                        lstbList[lstbList.Count - 1].ReqNo = Convert.ToInt32(dr["OATRequestId"].ToString());
                        lstbList[lstbList.Count - 1].EmpCode = dr["EmployeeCode"].ToString();
                        lstbList[lstbList.Count - 1].EmpName = (dr["FirstName"].ToString() +"  "+dr["LastName"].ToString());
                        lstbList[lstbList.Count - 1].Gender = dr["Gender"].ToString();
                        lstbList[lstbList.Count - 1].Designation = dr["Designation"].ToString();
                        lstbList[lstbList.Count - 1].Department = dr["Department"].ToString();
                        lstbList[lstbList.Count - 1].TravelDate = Convert.ToDateTime(dr["DepartureDate"].ToString());
                        lstbList[lstbList.Count - 1].AirlineName = dr["AirlineName"].ToString();
                        lstbList[lstbList.Count - 1].FlightNo = dr["FlightNumber"].ToString(); 
                        lstbList[lstbList.Count - 1].Sector = dr["OriginPlace"].ToString() + "-" + dr["DestinationPlace"].ToString();
                        lstbList[lstbList.Count - 1].Pnr = dr["PNR"].ToString(); 
                        if(dr["Amount"].ToString() != "")
                        {
                            lstbList[lstbList.Count - 1].Price = dr["Amount"].ToString();
                        }
                        else
                        {
                            lstbList[lstbList.Count - 1].Price = "0";
                        }
                        
                       lstbList[lstbList.Count - 1].IsNoShow =  dr["IsNoShow"].ToString();
                        

                        if (dr["CancelType"].ToString() == "c")
                        {
                            lstbList[lstbList.Count - 1].IsCancelled = "Cancel";
                        }
                        else
                        {
                            lstbList[lstbList.Count - 1].IsCancelled = "-----";
                        }
                        if(dr["CancelType"].ToString() == "frc")
                        {
                            lstbList[lstbList.Count - 1].IsFullRedundCancelled = "Full Refund Cancelled";
                        }
                        else
                        {
                            lstbList[lstbList.Count - 1].IsFullRedundCancelled = "----";
                        }
                        if(dr["HodApproval"].ToString() == "1")
                        {
                            lstbList[lstbList.Count - 1].HODApproval = "Yes";
                        }
                        
                        lstbList[lstbList.Count - 1].HODApprovalDate = dr["HodApprovalDate"].ToString();
                        
                    }
                }
                catch (Exception ex)
                {
                    var aa = ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstbList;
        }


        public static IList<BulkUploadMasterModels> GetFlightBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            var strConnString = ConnectionUtility.GetConnection();
            var lstBulkList = new List<BulkUploadMasterModels>();
            using (var con = new SqlConnection(strConnString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SP_GetOATBookingList"
                };
                cmd.Parameters.Add("@fromdate", SqlDbType.VarChar, 10).Value = fromdate;
                cmd.Parameters.Add("@todate", SqlDbType.VarChar, 10).Value = todate;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type; //contractual/non-contractual
                cmd.Parameters.Add("@criteria", SqlDbType.Int).Value = criteria;
                cmd.Connection = con;

                try
                {
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lstBulkList.Add(new BulkUploadMasterModels());
                        lstBulkList[lstBulkList.Count - 1].SNo = Convert.ToInt32(dr["SNo"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TRId = Convert.ToInt64(dr["TrnId"].ToString());
                        lstBulkList[lstBulkList.Count - 1].TravelDate = Convert.ToDateTime(dr["TravelDate"].ToString());
                        lstBulkList[lstBulkList.Count - 1].Sector = dr["Sector"].ToString();
                        lstBulkList[lstBulkList.Count - 1].EmpCode = dr["EmpCode"].ToString();
                        lstBulkList[lstBulkList.Count - 1].HodApproval = dr["HodApprovalStatus"].ToString();
                        //lstBulkList[lstBulkList.Count - 1].FileStatus = dr["FileStatus"].ToString();
                       // lstBulkList[lstBulkList.Count - 1].HotelStatus = Convert.ToInt16(dr["HotelStatus"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return lstBulkList;
        }
    }
}