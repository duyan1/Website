using EWShop.Models;
using EWShop.Utilities;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using System.Web;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace EWShop.Areas.Admin.Models
{
    public class Material
    {
        public string materialId { get; set; }
        public string materialCode { get; set; }
        public string materialDescription { get; set; }
        public string model { get; set; }
        public string plantID { get; set; }
        public string plantCode { get; set; }
        public string plantName { get; set; }
        
    }
    public class MaterialExcel
    {
        public string ID { get; set; }
        public string MATID { get; set; }
        public string MATName { get; set; }
        public string ModelType { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Currency { get; set; }
        public string FrmDate { get; set; }
        public string ToDate { get; set; }
        public string Flat { get; set; }
        public string Remarks { get; set; }
    }
    public static class  MaterialLogics {
        public static List<Material> getListMaterials(string plantId)
        {
            List<Material> list = new List<Material>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_MATERIAL))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (plantId != null && plantId != "")
                        cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = Convert.ToInt32(plantId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Material dt = new Material();
                                dt.materialCode = sdr["MATCODE"].ToString();
                                dt.materialDescription = sdr["MATDESCRIPTION"].ToString();
                                dt.materialId = sdr["MATID"].ToString();
                                dt.model = sdr["MTNAME"].ToString();
                                dt.plantCode = sdr["PLANTCODE"].ToString();
                                dt.plantID = sdr["PLANTID"].ToString();
                                dt.plantName = sdr["PLANTNAME"].ToString();
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return list;
        }
        public static Material getMaterial(string materialId)
        {
            Material dt = new Material();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_MATERIAL))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (materialId != null && !"".Equals(materialId))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = Convert.ToInt32(materialId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                dt.materialCode = sdr["MATCODE"].ToString();
                                dt.materialDescription = sdr["MATDESCRIPTION"].ToString();
                                dt.materialId = sdr["MATID"].ToString();
                                dt.model = sdr["MODELTYPEID"].ToString();
                                dt.plantID = sdr["PLANTID"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return dt;
        }
        public static ResultReturn saveMaterialInfo(Material mat, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SAVE_MATERIAL))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = mat.materialId == null ? "0" : mat.materialId.Trim();
                    cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = mat.materialCode == null ? "" : mat.materialCode.Trim();
                    cmd.Parameters.Add("@MATDESCIPTION", SqlDbType.NVarChar).Value = mat.materialDescription == null ? "" : mat.materialDescription.Trim();
                    cmd.Parameters.Add("@MTID", SqlDbType.Int).Value = mat.model == null ? "0" : mat.model.Trim();
                    cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = mat.plantID == null ? "" : mat.plantID.Trim();
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.msg = ex.Message;
                    }
                }
            }
            return result;
        }
    }
}