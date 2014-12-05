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

namespace ZhuoHuaAPP
{
    [Activity(Label = "卓华软件", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
              Theme = "@android:style/Theme.NoTitleBar")]
    public class HomePage : Activity
    {
        LinearLayout linearLayout_Product = null;
        LinearLayout linearLayout_Sale = null;
        LinearLayout linearLayout_Finance = null;
        LinearLayout linearLayout_Query = null;

        TextView tdOrderCount = null;
        TextView tdOrderPrice = null;
        TextView tdProductCount = null;
        TextView tdSendOutCount = null;
        TextView tdStoreCount = null;
        TextView tdPayable = null;
        TextView tdReceivable = null;

        TextView tdNoCheckOrderCount = null;
        TextView tdPurchaseQuotationCount = null;
        TextView tdSaleQuotationCount = null;
        TextView tdSalebookCount = null;
        TextView idCompany = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.homepage);
            initview();
        }

        private void initview()
        {
            linearLayout_Product = this.FindViewById<LinearLayout>(Resource.Id.linearLayout_Product);
            linearLayout_Sale = this.FindViewById<LinearLayout>(Resource.Id.linearLayout_Sale);
            linearLayout_Finance = this.FindViewById<LinearLayout>(Resource.Id.linearLayout_Finance);
            linearLayout_Query = this.FindViewById<LinearLayout>(Resource.Id.linearLayout_Query);
            linearLayout_Product.Click += new EventHandler(linearLayout_Product_Click);
            linearLayout_Sale.Click += new EventHandler(linearLayout_Sale_Click);
            linearLayout_Finance.Click += new EventHandler(linearLayout_Finance_Click);
            linearLayout_Query.Click += new EventHandler(linearLayout_Query_Click);


            tdOrderCount = this.FindViewById<TextView>(Resource.Id.tdOrderCount);
            tdOrderPrice = this.FindViewById<TextView>(Resource.Id.tdOrderPrice);
            tdProductCount = this.FindViewById<TextView>(Resource.Id.tdProductCount);
            tdSendOutCount = this.FindViewById<TextView>(Resource.Id.tdSendOutCount);
            tdStoreCount = this.FindViewById<TextView>(Resource.Id.tdStoreCount);
            tdPayable = this.FindViewById<TextView>(Resource.Id.tdPayable);
            tdReceivable = this.FindViewById<TextView>(Resource.Id.tdReceivable);

            tdNoCheckOrderCount = this.FindViewById<TextView>(Resource.Id.tdNoCheckOrderCount);
            tdPurchaseQuotationCount = this.FindViewById<TextView>(Resource.Id.tdPurchaseQuotationCount);
            tdSaleQuotationCount = this.FindViewById<TextView>(Resource.Id.tdSaleQuotationCount);
            tdSalebookCount = this.FindViewById<TextView>(Resource.Id.tdSalebookCount);
            idCompany = this.FindViewById<TextView>(Resource.Id.idCompany);


            tdOrderCount.SetTextColor(Android.Graphics.Color.Black);
            tdOrderPrice.SetTextColor(Android.Graphics.Color.Black);
            tdProductCount.SetTextColor(Android.Graphics.Color.Black);
            tdSendOutCount.SetTextColor(Android.Graphics.Color.Black);
            tdStoreCount.SetTextColor(Android.Graphics.Color.Black);
            tdPayable.SetTextColor(Android.Graphics.Color.Black);
            tdReceivable.SetTextColor(Android.Graphics.Color.Black);
            tdNoCheckOrderCount.SetTextColor(Android.Graphics.Color.Black);
            tdPurchaseQuotationCount.SetTextColor(Android.Graphics.Color.Black);
            tdSaleQuotationCount.SetTextColor(Android.Graphics.Color.Black);
            tdSalebookCount.SetTextColor(Android.Graphics.Color.Black); 
  
            initData();
        }

        private void initData()
        {
            tdOrderCount.Text = string.Format("{0:N}", 1892);
            tdOrderPrice.Text = string.Format("{0:N}", 2900.00);
            tdProductCount.Text = string.Format("{0:N}", 1892);
            tdSendOutCount.Text = string.Format("{0:N}", 1892); ;
            tdStoreCount.Text = string.Format("{0:N}", 1892);
            tdPayable.Text = string.Format("{0:N}", 29200.00);
            tdReceivable.Text = string.Format("{0:N}", 1236790.00);

            tdNoCheckOrderCount.Text = string.Format("{0:N}", 1892);
            tdPurchaseQuotationCount.Text = string.Format("{0:N}", 1892);
            tdSaleQuotationCount.Text = string.Format("{0:N}", 1892);
            tdSalebookCount.Text = string.Format("{0:N}", 1892);
            idCompany.Text = "卓华软件移动平台";
            idCompany.Click += new EventHandler(idCompany_Click);
        }

        void idCompany_Click(object sender, EventArgs e)
        {
            Intent layOut = new Intent();
            layOut.SetClass(this, typeof(AboutUs));
            StartActivity(layOut);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(Menu.None, Menu.First + 1, 5, "删除").SetIcon(Android.Resource.Drawable.IcMenuDelete);
            menu.Add(Menu.None, Menu.First + 2, 2, "保存").SetIcon(Android.Resource.Drawable.IcMenuEdit);
            menu.Add(Menu.None, Menu.First + 3, 6, "帮助").SetIcon(Android.Resource.Drawable.IcMenuHelp);
            menu.Add(Menu.None, Menu.First + 4, 1, "添加").SetIcon(Android.Resource.Drawable.IcMenuAdd);
            menu.Add(Menu.None, Menu.First + 5, 4, "详细").SetIcon(Android.Resource.Drawable.IcMenuInfoDetails);
            menu.Add(Menu.None, Menu.First + 6, 3, "发送").SetIcon(Android.Resource.Drawable.IcMenuSend);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {

                case Menu.First + 1:
                    Toast.MakeText(this, "删除菜单被点击了", ToastLength.Long).Show(); 
                    break;

                case Menu.First + 2:
                    Toast.MakeText(this, "保存菜单被点击了", ToastLength.Long).Show(); 
                    break;

                case Menu.First + 3: 
                    Toast.MakeText(this, "帮助菜单被点击了", ToastLength.Long).Show(); 
                    break;

                case Menu.First + 4: 
                    Toast.MakeText(this, "添加菜单被点击了", ToastLength.Long).Show(); 
                    break;

                case Menu.First + 5: 
                    Toast.MakeText(this, "详细菜单被点击了", ToastLength.Long).Show(); 
                    break;

                case Menu.First + 6: 
                    Toast.MakeText(this, "发送菜单被点击了", ToastLength.Long).Show(); 
                    break; 
            } 
            return false;
        }
        public override void OnOptionsMenuClosed(IMenu menu)
        { 
        }
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            return true;
        }
        void linearLayout_Query_Click(object sender, EventArgs e)
        {  
            Intent layOut = new Intent();
            layOut.SetClass(this, typeof(webviewActivity));
            StartActivity(layOut);
        }

        void linearLayout_Finance_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, Resource.String.tv_title_Finance, ToastLength.Short).Show();
        }

        void linearLayout_Sale_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, Resource.String.tv_title_Sale, ToastLength.Short).Show();
        }

        void linearLayout_Product_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, Resource.String.tv_title_Product, ToastLength.Short).Show();
        }
    }
}