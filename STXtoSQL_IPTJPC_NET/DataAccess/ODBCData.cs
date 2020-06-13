using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<IPTJPC> Get_IPTJPC()
        {

            List<IPTJPC> lstIPTJPC = new List<IPTJPC>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select jpc_job_no,jpc_frm,jpc_grd,jpc_size,jpc_fnsh,jpc_wdth,jpc_ga_size, jpc_pcs,
                                    cast(jpc_msr/12 as int) as jpc_msr,cast(jpc_wgt as int) as jpc_wgt
                                    from iptjpc_rec
                                    where jpc_job_no in
                                    (select psh_job_no
                                    from iptpsh_rec s
                                    inner join iptjob_rec j
                                    on j.job_job_no = s.psh_job_no
                                    where s.psh_whs = 'SW'
                                    and psh_sch_seq_no <> 99999999
                                    and (job_job_sts = 0 or job_job_sts = 1)
                                    and (job_prs_cl = 'SL' or job_prs_cl = 'CL' or job_prs_cl = 'MB'))";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        IPTJPC b = new IPTJPC();

                        b.job_no = Convert.ToInt32(rdr["jpc_job_no"]);
                        b.frm = rdr["jpc_frm"].ToString();
                        b.grd = rdr["jpc_grd"].ToString();
                        b.size = rdr["jpc_size"].ToString();
                        b.fnsh = rdr["jpc_fnsh"].ToString();
                        b.wdth = Convert.ToDecimal(rdr["jpc_wdth"]);
                        b.ga_size = Convert.ToDecimal(rdr["jpc_ga_size"]);                    
                        b.pcs = Convert.ToInt32(rdr["jpc_pcs"]);
                        b.msr = Convert.ToInt32(rdr["jpc_msr"]);
                        b.wgt = Convert.ToInt32(rdr["jpc_wgt"]);

                        lstIPTJPC.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstIPTJPC;
        }
    }
}
