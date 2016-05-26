using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{

    public interface IDataContextFactory
    {
        System.Data.Linq.DataContext Context { get; }
        void SaveAll();
    }


    //public class DataContext : IDataContextFactory
    //{


    //    #region IDataContextFactory Members

    //    private System.Data.Linq.DataContext dt;

    //    public System.Data.Linq.DataContext Context
    //    {
    //        get
    //        {
    //            if (dt == null)
    //                dt = new Listing.Data.ListingDataContext();
    //            return dt;
    //        }
    //    }

    //    public void SaveAll()
    //    {
    //        dt.SubmitChanges();
    //    }

    //    #endregion
    //}

}
