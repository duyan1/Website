using EWShop.Areas.Admin.Models;
using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Admin.Logics
{
    public static class SurveyLogics
    {
        public static List<RecordItem> getListRecords(string key)
        {
            List<RecordItem> listRecords = new List<RecordItem>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Survey_GetListRecords"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@KEY", SqlDbType.VarChar).Value = key == null ? "Q" : key;
                    cmd.Connection = con;
                    
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                RecordItem item = new RecordItem();
                                item.ItemId = sdr["RecordID"].ToString();
                                item.ItemContent = sdr["RecordContent"].ToString();
                                listRecords.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        listRecords = new List<RecordItem>();
                    }
                }
            }
            return listRecords;
        }
        public static List<ItemCompleted> getListQuestions(string term)
        {
            List<ItemCompleted> result = new List<ItemCompleted>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Survey_GetListQuestions"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@KEY", SqlDbType.NVarChar).Value = term == null ? "" : term.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemCompleted item = new ItemCompleted();
                                item.id = sdr["QID"].ToString();
                                item.label = sdr["QCONTENT"].ToString();
                                item.value = sdr["QCONTENT"].ToString();
                                result.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<ItemCompleted>();
                    }
                }
            }
            return result;
        }
        public static QAItem executeQASurvey(string question, string answer, string srnum, string remarks)
        {
            QAItem result = new QAItem();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Survey_ExecuteQASurvey"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@QID", SqlDbType.Int).Value = question == null ? "0" : question;
                    cmd.Parameters.Add("@QAID", SqlDbType.Int).Value = answer == null ? "0" : answer;
                    cmd.Parameters.Add("@SRNUM", SqlDbType.VarChar).Value = srnum == null ? "0" : srnum;
                    if(remarks != null)
                        if("".Equals(remarks))
                            cmd.Parameters.Add("@REMARKS", SqlDbType.NVarChar).Value = remarks == null ? "" : remarks;
                    cmd.Connection = con;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.question = new RecordItem();
                                result.question.ItemId = sdr["QID"].ToString();
                                result.question.ItemContent = sdr["QCONTENT"].ToString();
                                result.answers = new List<AnswerItem>();
                            }
                            sdr.NextResult();
                            while (sdr.Read())
                            {
                                AnswerItem item = new AnswerItem();
                                item.QAId = sdr["QAID"].ToString();
                                item.AId = sdr["AID"].ToString();
                                item.AContent = sdr["ACONTENT"].ToString();
                                item.Priority = sdr["PRIORITY"].ToString();
                                item.QIdNext = sdr["QNEXTID"].ToString();
                                item.QContentNext = sdr["QNEXTCONTENT"].ToString();
                                item.isCheck = sdr["ISREMARKS"].ToString();
                                result.answers.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
            }
            return result;
        }
        public static QAItem getDataQAItem(string QId)
        {
            QAItem result = new QAItem();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Survey_GetQARecord"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@QID", SqlDbType.Int).Value = QId == null ? "0" : QId;
                    cmd.Connection = con;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.question = new RecordItem();
                                result.question.ItemId = sdr["QID"].ToString();
                                result.question.ItemContent = sdr["QCONTENT"].ToString();
                                result.answers = new List<AnswerItem>();
                            }
                            sdr.NextResult();
                            while (sdr.Read())
                            {
                                AnswerItem item = new AnswerItem();
                                item.QAId = sdr["QAID"].ToString();
                                item.AId = sdr["AID"].ToString();
                                item.AContent = sdr["ACONTENT"].ToString();
                                item.Priority = sdr["PRIORITY"].ToString();
                                item.QIdNext = sdr["QNEXTID"].ToString();
                                item.QContentNext = sdr["QNEXTCONTENT"].ToString();
                                item.isCheck = sdr["ISREMARKS"].ToString();
                                result.answers.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
            }
            return result;
        }
    }
}