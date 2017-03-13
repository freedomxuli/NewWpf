using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIShell.OSGi;
using UIShell.OSGi.Utility;

namespace UIShell.NavigationService
{
    /// <summary>
    /// 代表由插件注册的导航节点。每一个插件可以在扩展下注册多个顶级节点。每一个节点可以包含无限制的子节点。
    /// </summary>
    public class NavigationNode : IComparable
    {
        public IBundle Bundle { get; set; }

        public string ParentId { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// 导航节点显示信息。
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 导航节点对应的值。
        /// </summary>
        public string Value { get; internal set; }
        /// <summary>
        /// 导航节点的顺序。
        /// </summary>
        public float Order { get; internal set; }
        /// <summary>
        /// 该节点的访问权限ID。
        /// </summary>
        public string Permission { get; internal set; }
        /// <summary>
        /// 一个中间节点是否可以访问，取决于当前用户是否有子节点的权限。
        /// </summary>
        internal List<string> ChildrenPermissions { get; set; }
        /// <summary>
        /// 子节点权限将递归初始化，为了提高性能，该属性用于表示是否已经初始化。
        /// </summary>
        internal bool ChildrenPermissionInitialized { get; set; }
        /// <summary>
        /// 导航图标。
        /// </summary>
        public string Icon { get; internal set; }
        /// <summary>
        /// 导航Tooltip。
        /// </summary>
        public string ToolTip { get; internal set; }
        /// <summary>
        /// 自定义属性，即其它没有在当前属性集合定义的属性会保存到该集合。
        /// </summary>
        public Dictionary<string, string> Attributes { get; internal set; }
        /// <summary>
        /// 父节点。
        /// </summary>
        public NavigationNode Parent { get; internal set; }
        /// <summary>
        /// 子节点。
        /// </summary>
        public SortedSet<NavigationNode> Children { get; internal set; }

        public NavigationNode()
        {
            Children = new SortedSet<NavigationNode>();
            Attributes = new Dictionary<string, string>();
            ChildrenPermissions = new List<string>();
        }
        /// <summary>
        /// 初始化父节点权限。
        /// </summary>
        internal void InitializeChildrenPermission()
        {
            lock(this)
            {
                // 如果初始化，则返回。
                if (ChildrenPermissionInitialized)
                {
                    return;
                }

                ChildrenPermissions.Clear();

                // 获取所有子节点的权限定义。
                foreach (var child in Children)
                {
                    // 子节点本身权限
                    if(!string.IsNullOrEmpty(child.Permission))
                    {
                        ChildrenPermissions.Add(child.Permission);
                    }
                    else
                    { 
                        // 子节点下属所有子节点权限，将递归获取。
                        child.InitializeChildrenPermission();
                        if (child.ChildrenPermissions.Count > 0)
                        {
                            ChildrenPermissions.AddRange(child.ChildrenPermissions);
                        }
                    }
                }
                // 已经初始化。
                ChildrenPermissionInitialized = true;
            }
        }

        //private bool _lastChildrenHasPermission = false;
        //private DateTime _lastChildrenPermissionCheckTime = DateTime.MinValue;
        //// TODO: 可以更改缓存时间来提高权限检查的性能。
        //private static TimeSpan ChildrenPermissionCheckInternalBySeconds = TimeSpan.FromSeconds(30);

        //private bool ChildrenPermissionCheckTimeout()
        //{
        //    return (DateTime.Now - _lastChildrenPermissionCheckTime) > ChildrenPermissionCheckInternalBySeconds;
        //}

        /// <summary>
        /// 当前用户是否具备访问权限。
        /// </summary>
        /// <returns>如果有权限，则范围true，否则范围false。</returns>
        public bool HasPermission()
        {
            // 如果没有安装权限服务，则直接返回false。
            if (!BundleActivator.PermissionServiceTracker.IsServiceAvailable)
            {
                return false;
            }

            if (string.IsNullOrEmpty(Permission)) // 如果当前节点无设置访问权限，则由子节点来决定是否有权限
            {
                if(Children.Count > 0)
                {
                    if(!ChildrenPermissionInitialized)
                    {
                        InitializeChildrenPermission();
                    }

                    if(ChildrenPermissions.Count == 0)
                    {
                        return true;
                    }

                    //if(!ChildrenPermissionCheckTimeout())
                    //{
                    //    return _lastChildrenHasPermission;
                    //}

                    // 检查子节点的权限
                    bool hasPermission = false;
                    foreach(var childPermission in ChildrenPermissions)
                    {
                        hasPermission = BundleActivator.PermissionServiceTracker.DefaultOrFirstService.Check(Bundle, childPermission);
                        if(hasPermission) // 有权限，则Break。
                        {
                            break;
                        }
                    }

                    //_lastChildrenPermissionCheckTime = DateTime.Now;
                    //_lastChildrenHasPermission = hasPermission;

                    return hasPermission;
                }

                return true;
            }

            // 如果当前节点设置访问权限，则直接检查。
            return BundleActivator.PermissionServiceTracker.DefaultOrFirstService.Check(Bundle, Permission);
        }

        /// <summary>
        /// 导航节点比较，用于根据Order来排序。
        /// </summary>
        /// <param name="obj">另一节点。</param>
        /// <returns>返回Order.CompareTo(other.Order)。</returns>
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is NavigationNode))
            {
                return 1;
            }
            NavigationNode other = obj as NavigationNode;
            if (Order == other.Order) // 如果Order相同，则比较插件名称。
            {
                if (Name.Equals(other.Name))
                {
                    return GetHashCode().CompareTo(other.GetHashCode());
                }
                else
                {
                    return Name.CompareTo(other.Name);
                }
            }
            else
            {
                return Order.CompareTo(other.Order); // 否则按照Order顺序排列。
            }
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is NavigationNode))
            {
                return false;
            }
            var other = obj as NavigationNode;
            return Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(Id) ? 0 : Id.GetHashCode();
        }
    }
}
