using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;

namespace ZhuoHuaAPP
{
	class ControlDataTable
	{
		public DataTable OrderDataTableByUp(DataTable thisTable,int columnNum)
		{
			DataTable table1 = new DataTable ();
			table1=thisTable.Copy();
			for(int i=0;i<table1.Rows.Count;i++)
			{
				for(int j=i+1;j<table1.Rows.Count;j++)
				{
					if (Convert.ToDecimal (table1.Rows [i] [columnNum]) < Convert.ToDecimal (table1.Rows [j] [columnNum]))
					{
						for(int k=0;k<table1.Columns.Count;k++)
						{
							object DR = new object ();
							DR = table1.Rows [i] [k];
							table1.Rows [i][k]=table1.Rows [j][k];
							table1.Rows [j][k]=DR;
						}
					}
				}
			}
			return table1;
		}
		public	DataTable OrderDataTableByDown(DataTable thisTable,int columnNum)
		{
			DataTable table1 = new DataTable ();
			table1=thisTable.Copy();
			for(int i=0;i<table1.Rows.Count;i++)
			{
				for(int j=i+1;j<table1.Rows.Count;j++)
				{
					if (Convert.ToDecimal (table1.Rows [i] [columnNum]) > Convert.ToDecimal (table1.Rows [j] [columnNum]))
					{
						for(int k=0;k<table1.Columns.Count;k++)
						{
							object DR = new object ();
							DR = table1.Rows [i] [k];
							table1.Rows [i][k]=table1.Rows [j][k];
							table1.Rows [j][k]=DR;
						}
					}
				}
			}
			return table1;
		}
        public static int GetMaxItemInTable(DataTable dt, int Column)
        {
			if (dt.Rows.Count == 0)
				return 0;
			decimal max = Convert.ToDecimal(dt.Rows[0][Column]);
            int Item = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dt.Rows[i][Column]) > max)
                {
                    Item = i;
                    max = Convert.ToDecimal(dt.Rows[i][Column]);
                }
            }
            return Item;
        }
		public static int GetMinItemInTable(DataTable dt, int Column)
		{
			if (dt.Rows.Count == 0)
				return 0;
			decimal min = Convert.ToDecimal(dt.Rows[0][Column]);
			int Item = 0;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Convert.ToDecimal(dt.Rows[i][Column]) < min)
				{
					Item = i;
					min = Convert.ToDecimal(dt.Rows[i][Column]);
				}
			}
			return Item;
		}

	}
}

