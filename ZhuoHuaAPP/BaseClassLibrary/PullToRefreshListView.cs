using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using ZhuoHuaAPP;

namespace PullToRefresh
{
	public class PullToRefreshListView : ListView, AbsListView.IOnScrollListener
	{
		#region 变量定义
		private static int TAP_TO_REFRESH = 1;
		private static int PULL_TO_REFRESH = 2;
		private static int RELEASE_TO_REFRESH = 3;
		public static int REFRESHING = 4;
		/// <summary>
		/// 刷新监听事件
		/// </summary>
		private OnRefreshListener _OnRefreshListener;
		/// <summary>
		/// 滚动监听事件
		/// </summary>
		private IOnScrollListener _OnScrollListener;
		/// <summary>
		/// 动态布局
		/// </summary>
		private LayoutInflater _Inflater;
		/// <summary>
		/// 刷新View
		/// </summary>
		private RelativeLayout _RefreshView;
		/// <summary>
		/// 下拉时文字
		/// </summary>
		private TextView _RefreshViewText;
		/// <summary>
		/// 箭头图片
		/// </summary>
		private ImageView _RefreshViewImage;
		/// <summary>
		/// 刷新时的滚动条
		/// </summary>
		private ProgressBar _RefreshViewProgress;
		/// <summary>
		/// 上升时文字
		/// </summary>
		private TextView _RefreshViewLastUpdated;
		/// <summary>
		/// 刷新状态
		/// </summary>
		private int _RefreshState;
		/// <summary>
		/// 当前滚动状态
		/// </summary>
		private Android.Widget.ScrollState _CurrentScrollState;        
		/// <summary>
		/// 下拉时动画
		/// </summary>
		private RotateAnimation _FlipAnimation;
		/// <summary>
		/// 上升时动画
		/// </summary>
		private RotateAnimation _ReverseFlipAnimation;
		/// <summary>
		/// 下拉View高度
		/// </summary>
		private int _RefreshViewHeight;
		/// <summary>
		/// 顶部间隔
		/// </summary>
		private int _RefreshOriginalTopPadding;
		/// <summary>
		/// 最后移动Y
		/// </summary>
		private int _LastMotionY;
		/// <summary>
		/// 是否弹回
		/// </summary>
		private bool _BounceHack;

		#endregion

		#region 构造函数
		public PullToRefreshListView(Context m_Context)
			: base(m_Context)
		{
			Initialize();
		}

		public PullToRefreshListView(Context m_Context, IAttributeSet m_AttributeSet) :
			base(m_Context, m_AttributeSet)
		{
			Initialize();
		}

		public PullToRefreshListView(Context m_Context, IAttributeSet m_AttributeSet, int m_DefStyle) :
			base(m_Context, m_AttributeSet, m_DefStyle)
		{
			Initialize();
		}
		#endregion

		/// <summary>
		/// 初始化控件信息
		/// </summary>
		private void Initialize()
		{
			//下拉时候动画设置
			_FlipAnimation = new RotateAnimation(0, -180,
			                                     Android.Views.Animations.Dimension.RelativeToSelf, 0.5f,
			                                     Android.Views.Animations.Dimension.RelativeToSelf, 0.5f);
			_FlipAnimation.Interpolator = new LinearInterpolator();
			_FlipAnimation.Duration = 250;
			_FlipAnimation.FillAfter = true;

			//上升时动画设置
			_ReverseFlipAnimation = new RotateAnimation(-180, 0,
			                                            Android.Views.Animations.Dimension.RelativeToSelf, 0.5f,
			                                            Android.Views.Animations.Dimension.RelativeToSelf, 0.5f);
			_ReverseFlipAnimation.Interpolator = new LinearInterpolator();
			_ReverseFlipAnimation.Duration = 250;
			_ReverseFlipAnimation.FillAfter = true;

			//动态获取布局中的各个组件
			_Inflater = Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
			_RefreshView = (RelativeLayout)_Inflater.Inflate(Resource.Layout.pull_to_refresh_header, this, false);
			_RefreshViewText = _RefreshView.FindViewById<TextView>(Resource.Id.pull_to_refresh_text);
			_RefreshViewImage = _RefreshView.FindViewById<ImageView>(Resource.Id.pull_to_refresh_image);
			_RefreshViewProgress = _RefreshView.FindViewById<ProgressBar>(Resource.Id.pull_to_refresh_progress);
			_RefreshViewLastUpdated = _RefreshView.FindViewById<TextView>(Resource.Id.pull_to_refresh_updated_at);
			_RefreshViewImage.SetMinimumHeight(50);

			//其他控件设置
			_RefreshView.SetOnClickListener(new OnClickRefreshListener(this));
			_RefreshOriginalTopPadding = _RefreshView.PaddingTop;
			_RefreshState = TAP_TO_REFRESH;
			this.AddHeaderView(_RefreshView);
			base.SetOnScrollListener(this);
			this.MeasureView(_RefreshView);
			_RefreshViewHeight = _RefreshView.MeasuredHeight;
		}

		#region 重载方法
		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			SetSelection(1);
		}

		public override IListAdapter Adapter
		{
			set
			{
				base.Adapter = value;
				SetSelection(1);
			}
		}

		public override void SetOnScrollListener(IOnScrollListener m_IOnScrollListener)
		{
			_OnScrollListener = m_IOnScrollListener;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			int Y = (int)e.GetY();
			_BounceHack = false;
			switch (e.Action)
			{
				case MotionEventActions.Up:
				if (!VerticalScrollBarEnabled)
					VerticalScrollBarEnabled = true;
				if (FirstVisiblePosition == 0 && _RefreshState != REFRESHING)
				{
					if ((_RefreshView.Bottom >= _RefreshViewHeight || _RefreshView.Top >= 0) && _RefreshState == RELEASE_TO_REFRESH)
					{
						_RefreshState = REFRESHING;
						PrepareForRefresh();
						OnRefresh();
					}
					else if (_RefreshView.Bottom < _RefreshViewHeight || _RefreshView.Top <= 0)
					{
						ResetHeader();
						SetSelection(1);
					}
				}
				break;
				case MotionEventActions.Down:
				_LastMotionY = Y;
				break;
				case MotionEventActions.Move:
				ApplyHeaderPadding(e);
				break;
			}
			return base.OnTouchEvent(e);
		}

		#endregion        

		public void SetOnRefreshListener(OnRefreshListener m_Listener)
		{
			_OnRefreshListener = m_Listener;
		}

		public void SetLastUpdated(string m_LastUpdated)
		{
			if (!string.IsNullOrEmpty(m_LastUpdated))
			{
				_RefreshViewLastUpdated.Visibility = ViewStates.Visible;
				_RefreshViewLastUpdated.Text = m_LastUpdated;
			}
			else
				_RefreshViewLastUpdated.Visibility = ViewStates.Gone;
		}



		private void ApplyHeaderPadding(MotionEvent ev)
		{
			int pointerCount = ev.HistorySize;
			for (int p = 0; p < pointerCount; p++)
			{
				if (_RefreshState == RELEASE_TO_REFRESH)
				{
					if (VerticalFadingEdgeEnabled)
						VerticalScrollBarEnabled = false;

					int historicalY = (int)ev.GetHistoricalY(p);
					int topPadding = (int)(((historicalY - _LastMotionY)
					                        - _RefreshViewHeight) / 1.7);

					_RefreshView.SetPadding(
						_RefreshView.PaddingLeft,
						topPadding,
						_RefreshView.PaddingRight,
						_RefreshView.PaddingBottom);
				}
			}
		}

		private void ResetHeaderPadding()
		{
			_RefreshView.SetPadding(_RefreshView.PaddingLeft,_RefreshOriginalTopPadding,_RefreshView.PaddingRight,_RefreshView.PaddingBottom);
		}

		private void ResetHeader()
		{
			if (_RefreshState != TAP_TO_REFRESH)
			{
				_RefreshState = TAP_TO_REFRESH;
				ResetHeaderPadding();                
				_RefreshViewText.SetText(Resource.String.pull_to_refresh_tap_label);                
				_RefreshViewImage.SetImageResource(Resource.Drawable.ic_pulltorefresh_arrow);                
				_RefreshViewImage.ClearAnimation();               
				_RefreshViewImage.Visibility = ViewStates.Gone;
				_RefreshViewProgress.Visibility = ViewStates.Gone;
			}
		}
		private void MeasureView(View m_Child)
		{
			ViewGroup.LayoutParams p = m_Child.LayoutParameters;
			if (p == null)
				p = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
			int childWidthSpec = ViewGroup.GetChildMeasureSpec(0, 0 + 0, p.Width);
			int lpHeight = p.Height;
			int childHeightSpec;
			if (lpHeight > 0)
			{
				childHeightSpec = MeasureSpec.MakeMeasureSpec(lpHeight, MeasureSpecMode.Exactly);
			}
			else
			{
				childHeightSpec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
			}
			m_Child.Measure(childWidthSpec, childHeightSpec);
		}

		public void OnScroll(AbsListView m_View, int m_FirstVisibleItem, int m_VisibleItemCount, int m_TotalItemCount)
		{
			if (_CurrentScrollState == Android.Widget.ScrollState.TouchScroll && _RefreshState != REFRESHING)
			{
				if (m_FirstVisibleItem == 0)
				{
					_RefreshViewImage.Visibility = ViewStates.Visible;
					if ((_RefreshView.Bottom >= _RefreshViewHeight + 20
					     || _RefreshView.Top >= 0)
					    && _RefreshState != RELEASE_TO_REFRESH)
					{
						_RefreshViewText.SetText(Resource.String.pull_to_refresh_release_label);
						_RefreshViewImage.ClearAnimation();
						_RefreshViewImage.StartAnimation(_FlipAnimation);
						_RefreshState = RELEASE_TO_REFRESH;
					}
					else if (_RefreshView.Bottom < _RefreshViewHeight + 20
					         && _RefreshState != PULL_TO_REFRESH)
					{
						_RefreshViewText.SetText(Resource.String.pull_to_refresh_pull_label);
						if (_RefreshState != TAP_TO_REFRESH)
						{
							_RefreshViewImage.ClearAnimation();
							_RefreshViewImage.StartAnimation(_ReverseFlipAnimation);
						}
						_RefreshState = PULL_TO_REFRESH;
					}
				}
				else
				{
					_RefreshViewImage.Visibility = ViewStates.Gone;
					ResetHeader();
				}
			}
			else if (_CurrentScrollState == ScrollState.Fling
			         && m_FirstVisibleItem == 0
			         && _RefreshState != REFRESHING)
			{
				SetSelection(1);
				_BounceHack = true;
			}
			else if (_BounceHack && _CurrentScrollState == ScrollState.Fling)
			{
				SetSelection(1);
			}

			if (_OnScrollListener != null)
			{
				_OnScrollListener.OnScroll(m_View, m_FirstVisibleItem, m_VisibleItemCount, m_TotalItemCount);
			}
		}

		public void OnScrollStateChanged(AbsListView m_View, ScrollState m_ScrollState)
		{
			_CurrentScrollState = m_ScrollState;

			if (_CurrentScrollState == ScrollState.Idle)
			{
				_BounceHack = false;
			}

			if (_OnScrollListener != null)
			{
				_OnScrollListener.OnScrollStateChanged(m_View, m_ScrollState);
			}
		}

		public void PrepareForRefresh()
		{
			ResetHeaderPadding();
			_RefreshViewImage.Visibility = ViewStates.Gone;
			_RefreshViewImage.SetImageDrawable(null);
			_RefreshViewProgress.Visibility = ViewStates.Visible;
			_RefreshViewText.SetText(Resource.String.pull_to_refresh_refreshing_label);
			_RefreshState = REFRESHING;
		}

		public void OnRefresh()
		{
			if (_OnRefreshListener != null)
			{
				_OnRefreshListener.OnRefresh();
			}
		}

		public void OnRefreshComplete(string m_LastUpdated)
		{
			SetLastUpdated(m_LastUpdated);
			OnRefreshComplete();
		}

		public void OnRefreshComplete()
		{
			ResetHeader();
			if (_RefreshView.Bottom > 0)
			{
				InvalidateViews();
				SetSelection(1);
			}
		}

		private class OnClickRefreshListener : Java.Lang.Object, IOnClickListener
		{
			PullToRefreshListView listView;
			public OnClickRefreshListener(PullToRefreshListView m_Lv)
			{
				listView = m_Lv;
			}

			public void OnClick(View m_View)
			{
				if (listView._RefreshState != PullToRefreshListView.REFRESHING)
				{
					listView.PrepareForRefresh();
					listView.OnRefresh();
				}
			}
		}

		public interface OnRefreshListener
		{
			void OnRefresh();
		}
    }
}