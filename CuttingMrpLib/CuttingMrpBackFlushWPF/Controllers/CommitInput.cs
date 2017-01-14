using CuttingMrpBackFlushWPF.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CuttingMrpBackFlushWPF.Controllers
{
    public class Message
    {
        public bool result { get; set; }
        public string message { get; set; }

        public Message() { }

        public Message(bool result, string messgae)
        {
            this.result = result;
            this.message = messgae;
        }
    }
    class CommitInput
    {
        ProductFinishDataContext context = new ProductFinishDataContext();
        /// <summary>
        /// 验证输入是否正确
        /// </summary>
        /// <param name="productNr"></param>
        /// <param name="bomId"></param>
        /// <returns></returns>
        public Message CheckInput(string productNr, string partNr)
        {
            try
            {
                
                if (string.IsNullOrEmpty(productNr))
                {
                    return new Message(false, "唯一码不能为空！");
                }
                else if (productNr.Length != Properties.Settings.Default.productNrLength)
                {
                    return new Message(false, "唯一码不正确！");
                }

                if (string.IsNullOrEmpty(partNr))
                {
                    return new Message(false, "配置号不能为空！");
                }

                if (context.ProductFinish.FirstOrDefault(s => s.productnr == productNr) != null)
                {
                    return new Message(false, "当前唯一码已存在，请确认是否正确！");
                }
                if (context.Part.FirstOrDefault(p => p.partNr == partNr) == null)
                {
                    return new Message(false, "当前配置号不存在，请确认是否正确！");
                }
                else
                {
                    return new Message(true, "");

                }
            }
            catch(Exception e)
            {
                return new Message(false,"异常信息："+e.Message);
            }
            
        }
        /// <summary>
        /// 提交到数据库，插入ProductFinish表
        /// </summary>
        /// <param name="productNr"></param>
        /// <param name="partNr"></param>
        /// <returns></returns>
        public Message Commit(string productNr, string partNr)
        {
            //ProductFinishDataContext context = new ProductFinishDataContext();
            try
            {
                ProductFinish productFinish = new ProductFinish();
                productFinish.productnr = productNr;
                productFinish.partNr = partNr;
                productFinish.status = 2000;
                productFinish.finishTime = DateTime.Now;
                context.ProductFinish.InsertOnSubmit(productFinish);
                //context.SubmitChanges();

                MPS mps = new MPS();
                mps.partnr = partNr;
                mps.orderedDate = DateTime.Now;
                mps.requiredDate = DateTime.Now;
                mps.quantity = 1;
                mps.source = "KSK";
                mps.sourceDoc = "KSK";
                mps.status = 2000;
                mps.unitId = null;
                context.MPS.InsertOnSubmit(mps);
                context.SubmitChanges();

                return new Message(true, "添加成功！");
            }
            catch (SqlException e)
            {
                //MessageBox.Show(e.Message);
                return new Message(false, "异常信息："+e.Message);
            }
        }
    }
}
