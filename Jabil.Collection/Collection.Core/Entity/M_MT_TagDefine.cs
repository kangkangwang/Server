using System.Collections.Generic;
using DeerMonitor.Base.Core.Entity;

namespace Collection.Core.Model
{
    public class M_MT_TagDefine : M_BS_TagDefine
    {
        private List<M_BS_TagDefine> _lstTags = new List<M_BS_TagDefine>();

        public List<M_BS_TagDefine> lstTags
        {
            get { return _lstTags; }
            set { _lstTags = value; }
        }
    }
}