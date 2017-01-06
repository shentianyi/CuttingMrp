using CuttingMrpBackFlushWPF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingMrpBackFlushWPF.Controllers
{
    class ShowData
    {

        /// <summary>
        /// 操作数据库搜索数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="productNr"></param>
        /// <param name="partNr"></param>
        public List<ProductFinish> SearchData(DateTime startDate, DateTime endDate, string productNr, string partNr)
        {
            ProductFinishDataContext context = new ProductFinishDataContext();
            List<ProductFinish> list = new List<ProductFinish>();
            if (string.IsNullOrEmpty(productNr) && string.IsNullOrEmpty(partNr))
            {
                list = (from p in context.ProductFinish
                        where p.finishTime >= startDate && p.finishTime <= endDate
                        select p).ToList();
            }
            else if (!string.IsNullOrEmpty(productNr) && !string.IsNullOrEmpty(partNr))
            {
                list = (from p in context.ProductFinish
                        where p.finishTime >= startDate && p.finishTime <= endDate
                        && p.productnr == productNr && p.partNr == partNr
                        select p).ToList();
            }
            else if (string.IsNullOrEmpty(productNr) && !string.IsNullOrEmpty(partNr))
            {
                list = (from p in context.ProductFinish
                        where p.finishTime >= startDate && p.finishTime <= endDate
                        && p.partNr == partNr
                        select p).ToList();
            }
            else if (!string.IsNullOrEmpty(productNr) && string.IsNullOrEmpty(partNr))
            {
                list = (from p in context.ProductFinish
                        where p.finishTime >= startDate && p.finishTime <= endDate
                        && p.productnr == productNr
                        select p).ToList();
            }
            return list;
           
        }
    }
}
