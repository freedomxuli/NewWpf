using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIShell.OSGi;
using UIShell.OSGi.Collection;
using UIShell.OSGi.Utility;

namespace UIShell.NavigationService
{
    /// <summary>
    /// 默认导航扩展服务实现。
    /// </summary>
    class NavigationServiceImpl : INavigationService
    {
        /// <summary>
        /// 导航扩展点名称。
        /// </summary>
        public readonly string NavigationExtensionPoint;
        /// <summary>
        /// 导航扩展变更事件。
        /// </summary>
        public event EventHandler<NavigationChangedEventArgs> NavigationChanged;
        /// <summary>
        /// 所有导航信息，按照插件进行分类，每一个插件使用一个Navigation来保存其定义的所有导航节点。
        /// </summary>
        private ThreadSafeList<NavigationNode> NavigationNodeList { get; set; }
        /// <summary>
        /// 插件上下文。
        /// </summary>
        private IBundleContext _context;
        private SortedSet<NavigationNode> PendingAppendedNodes { get; set; }

        public NavigationServiceImpl(IBundleContext context, string navigationExtensionPoint = "UIShell.NavigationService")
        {
            NavigationExtensionPoint = navigationExtensionPoint;
            _context = context;
            NavigationNodeList = new ThreadSafeList<NavigationNode>();
            PendingAppendedNodes = new SortedSet<NavigationNode>();
            InitializeExtensions();
            _context.ExtensionChanged += OnNavigationExtensionChanged;
        }

        /// <summary>
        /// 所有插件注册的导航节点信息。
        /// </summary>
        public SortedSet<NavigationNode> NavigationNodes
        {
            get
            {
                var result = new SortedSet<NavigationNode>();
                NavigationNodeList.ForEach(node =>
                    {
                        result.Add(node);
                    });
                return result;
            }
        }

        private void InitializeExtensions() // 初始化扩展信息。
        {
            NavigationNodeList.Clear();
            PendingAppendedNodes.Clear();
            // 获取该扩展点的所有扩展信息。
            var extensions = _context.GetExtensions(NavigationExtensionPoint);
            // 将扩展XML信息转换成扩展模型。
            extensions.ForEach(extension => AddNavigationByExtension(extension));
            HandlePendingAppendedNodes();
        }

        private void HandlePendingAppendedNodes()
        {
            if(PendingAppendedNodes.Count == 0)
            {
                return;
            }

            var nodesStack = new List<NavigationNode>();
            NavigationNodeList.ForEach(navNode => nodesStack.Add(navNode));
            PendingAppendedNodes.ToList().ForEach(navNode => nodesStack.Add(navNode));

            List<NavigationNode> currentStack;
            NavigationNode current;
            foreach(var node in PendingAppendedNodes.ToArray())
            {
                currentStack = new List<NavigationNode>(nodesStack.ToArray());
                while(currentStack.Count > 0)
                {
                    current = currentStack[0];
                    currentStack.RemoveAt(0);

                    if (node.ParentId.Equals(current.Id))
                    {
                        current.Children.Add(node);
                        node.Parent = current;
                        PendingAppendedNodes.Remove(node);
                        NavigationNodeList.Remove(node);
                        continue;
                    }
                    foreach(var child in current.Children)
                    {
                        currentStack.Add(child);
                    }
                }
            }

            foreach(var node in PendingAppendedNodes)
            {
                using (var locker = NavigationNodeList.Lock())
                {
                    if(locker.Contains(node))
                    {
                        continue;
                    }
                }
                NavigationNodeList.Add(node);
            }
        }

        /// <summary>
        /// 扩展变更处理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigationExtensionChanged(object sender, ExtensionEventArgs e)
        {
            // 当插件框架停止时，忽略扩展变化
            if(BundleRuntime.Instance.State == BundleRuntimeState.Stopping)
            {
                return;
            }
            if (e.ExtensionPoint.Equals(NavigationExtensionPoint))
            {
                if (e.Action == CollectionChangedAction.Add) // 扩展XML信息增加，即插件启动。
                {
                    AddNavigationByExtension(e.Extension);
                    HandlePendingAppendedNodes();
                }
                else // 扩展XML信息删除，即插件停止。
                {
                    RemoveNavigationByExtension(e.Extension);
                }

                if (NavigationChanged != null) // 发出扩展变更事件。
                {
                    NavigationChanged(this, new NavigationChangedEventArgs { Action = CollectionChangedAction.Add, Bundle = e.Extension.Owner });
                }
            }
        }

        /// <summary>
        /// 将一个Extension对象转换成Navigation对象。Extension对象存储的扩展信息是XML格式的，Navigation则是
        /// 对象模型。
        /// </summary>
        /// <param name="extension">扩展对象。</param>
        private void AddNavigationByExtension(Extension extension)
        {
            foreach (XmlNode node in extension.Data) // 遍历扩展XML节点。
            {
                if (node is XmlComment)
                {
                    continue;
                }
                // 将XML节点转换成导航节点。
                ConvertToNode(null, node, extension.Owner);
            }
        }

        /// <summary>
        /// 将Xml节点转换成导航节点对象。
        /// </summary>
        /// <param name="nav">所属Navigation。</param>
        /// <param name="node">Xml节点。</param>
        /// <param name="Parent">父导航节点。</param>
        /// <param name="children">子导航节点列表。</param>
        private void ConvertToNode(NavigationNode parent, XmlNode node, IBundle bundle)
        {
            // 获取Id、Name、Value、Order、Permisson、Icon和Tooltip属性。
            string id = GetAttribute(node, "Id");
            string name = GetAttribute(node, "Name");
            if (string.IsNullOrEmpty(name))
            {
                FileLogUtility.Warn("The Name attribute can not be empty for UIShell.NavigationService extension.");
                return;
            }
            string value = GetAttribute(node, "Value");
            string order = GetAttribute(node, "Order");
            float orderFloat = 0;
            float.TryParse(order, out orderFloat);
            string permission = GetAttribute(node, "Permission");
            string icon = GetAttribute(node, "Icon");
            string toolTip = GetAttribute(node, "Tooltip");
            string parentId = GetAttribute(node, "ParentId");
            // 创建导航节点。
            var navNode = new NavigationNode { 
                ParentId = string.IsNullOrEmpty(parentId) ? Guid.NewGuid().ToString() : parentId, 
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id, 
                Name = name, 
                Order = orderFloat,
                Value = value,
                Permission = permission,
                Icon = icon,
                ToolTip = toolTip,
                Bundle = bundle
            };
            // 设置父节点，并添加到子节点列表。

            if(!string.IsNullOrEmpty(parentId))
            {
                PendingAppendedNodes.Add(navNode);
            }
            else if(string.IsNullOrEmpty(parentId))
            {
                if(parent != null)
                {
                    navNode.Parent = parent;
                    parent.Children.Add(navNode);
                }
                else
                {
                    navNode.Parent = null;
                    navNode.ParentId = Guid.NewGuid().ToString();
                    NavigationNodeList.Add(navNode);
                }
            }

            // 将XML节点其它的属性保存到Attributes字典。
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name.Equals("Id") || attr.Name.Equals("Name") || attr.Name.Equals("Value") || attr.Name.Equals("Order") || attr.Name.Equals("Permission") || attr.Name.Equals("Icon") || attr.Name.Equals("ToolTip"))
                {
                    continue;
                }
                navNode.Attributes[attr.Name] = attr.Value;
            }
            // 遍历Xml子节点，并递归转换成导航节点。
            foreach (XmlNode childnode in node.ChildNodes)
            {
                if (childnode is XmlComment)
                {
                    continue;
                }
                ConvertToNode(navNode, childnode, bundle);
            }
        }

        /// <summary>
        /// 用非递归方法删除当前插件的节点。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <param name="bundle">当前插件。</param>
        private void RemoveNode(NavigationNode parent, IBundle bundle)
        {
            var stack = new List<NavigationNode>();
            stack.Add(parent);
            NavigationNode current;
            while (stack.Count > 0)
            {
                current = stack[0];
                stack.RemoveAt(0);

                foreach(var node in current.Children.ToArray())
                {
                    if(node.Bundle.Equals(bundle))
                    {
                        current.Children.Remove(node);
                    }
                    else
                    {
                        stack.Add(node);
                    }
                }
            }
        }

        /// <summary>
        /// 删除扩展模型。
        /// </summary>
        /// <param name="extension">扩展对象。</param>
        private void RemoveNavigationByExtension(Extension extension)
        {
            NavigationNodeList.RemoveAll(node => node.Bundle.Equals(extension.Owner));

            NavigationNodeList.ForEach(node =>
                {
                    RemoveNode(node, extension.Owner);
                });
        }

        /// <summary>
        /// 获取一个XML节点的属性。
        /// </summary>
        /// <param name="node">XML节点。</param>
        /// <param name="attribute">属性名称。</param>
        /// <returns>属性值。</returns>
        private string GetAttribute(XmlNode node, string attribute)
        {
            var attr = node.Attributes[attribute];
            if (attr != null)
            {
                return attr.Value.Trim();
            }
            return string.Empty;
        }
    }

    public class NavigationServiceFactory : INavigationServiceFactory
    {
        private ThreadSafeDictionary<string, INavigationService> _serviceCache = new ThreadSafeDictionary<string,INavigationService>();
        private IBundleContext _context;
        public NavigationServiceFactory(IBundleContext context)
        {
            AssertUtility.NotNull(context, "BundleContext");
            _context = context;
        }

        public INavigationService CreateNavigationService(string extensionPoint)
        {
            using(var locker = _serviceCache.Lock())
            {
                if(!locker.ContainsKey(extensionPoint))
                {
                    locker[extensionPoint] = new NavigationServiceImpl(_context, extensionPoint);
                }
                return locker[extensionPoint];
            }
        }

        public void DisposeNavigationService(string extensionPoint)
        {
            _serviceCache.Remove(extensionPoint);
        }
    }
}
