using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    class SQLData : Helpers
    {
        // Insert list of IPTJPC from STRATIX into IMPORT
        public int Write_IPTJPC_IMPORT(List<IPTJPC> lstIPTJPC)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from ST_IMPORT_tbl_IPTJPC";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = "INSERT INTO ST_IMPORT_tbl_IPTJPC (jpc_job_no,jpc_frm,jpc_grd,jpc_size,jpc_fnsh,jpc_wdth,jpc_ga_size, jpc_pcs,jpc_msr,jpc_wgt) " +
                                        "VALUES (@arg1,@arg2,@arg3,@arg4,@arg5,@arg6,@arg7,@arg8,@arg9,@arg10)";

                    cmd.Parameters.Add("@arg1", SqlDbType.Int);
                    cmd.Parameters.Add("@arg2", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg3", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg4", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg5", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg6", SqlDbType.Decimal);
                    cmd.Parameters.Add("@arg7", SqlDbType.Decimal);
                    cmd.Parameters.Add("@arg8", SqlDbType.Int);
                    cmd.Parameters.Add("@arg9", SqlDbType.Int);
                    cmd.Parameters.Add("@arg10", SqlDbType.Int);

                    foreach (IPTJPC s in lstIPTJPC)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(s.job_no);
                        cmd.Parameters[1].Value = s.frm;
                        cmd.Parameters[2].Value = s.grd;
                        cmd.Parameters[3].Value = s.size;
                        cmd.Parameters[4].Value = s.fnsh;
                        cmd.Parameters[5].Value = Convert.ToDecimal(s.wdth);
                        cmd.Parameters[6].Value = Convert.ToDecimal(s.ga_size);
                        cmd.Parameters[7].Value = Convert.ToInt32(s.pcs);
                        cmd.Parameters[8].Value = Convert.ToInt32(s.msr);
                        cmd.Parameters[9].Value = Convert.ToInt32(s.wgt);                       

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't lave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(jpc_job_no) from ST_IMPORT_tbl_IPTJPC";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        // Insert values from IMPORT into WIP IPTJPC
        public int Write_IMPORT_to_IPTJPC()
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to IPTJPC table.  Return rows inserted.
                cmd.CommandText = "ST_proc_IMPORT_to_IPTJPC";
              
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        } 
    }
}
