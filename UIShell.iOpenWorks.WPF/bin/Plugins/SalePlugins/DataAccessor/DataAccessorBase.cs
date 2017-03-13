using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SalePlugins.Model;
using UIShell.OSGi.Utility;

namespace SalePlugins.DataAccessor
{
    /// <summary>
    /// 基础数据库访问类，定义了CRUD基础方法。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public abstract class DataAccessorBase<T> where T : class
    {
        /// <summary>
        /// 在数据库新增一个实体。
        /// </summary>
        /// <param name="entity">实体对象。</param>
        /// <returns>如果保存成功返回true，否则返回false。</returns>
        public virtual bool Create(T entity)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    // 将实体状态设置为增加。
                    context.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
                    // 保存实体变更。
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to create '{0}'.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 从数据库获取所有的实体。
        /// </summary>
        /// <returns>实体集合。</returns>
        public virtual List<T> GetAll()
        {
            try
            {
                using (var context = NewDbContext())
                {
                    return context.Set<T>().ToList(); // 获取所有实体。
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to get '{0}'s.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return new List<T>();
            }
        }

        /// <summary>
        /// 按照主键查询，以分页方式来获取实体列表。
        /// </summary>
        /// <typeparam name="TKey">实体类型。</typeparam>
        /// <param name="keySelector">主键查询条件表达式。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageNum">页码。</param>
        /// <param name="totalCount">总记录数。</param>
        /// <param name="pageCount">页面数。</param>
        /// <returns>当前页码的实体集合。</returns>
        public virtual List<T> GetPage<TKey>(Expression<Func<T, TKey>> keySelector, int pageSize, ref int pageNum, out int totalCount, out int pageCount)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    totalCount = context.Set<T>().Count(); // 所有记录

                    if (totalCount % pageSize == 0) // 获取页数
                    {
                        pageCount = totalCount / pageSize;
                    }
                    else
                    {
                        pageCount = totalCount / pageSize + 1;
                    }

                    if (pageNum <= 1) // 重置页码
                    {
                        pageNum = 1;
                    }
                    if (pageNum >= pageCount)
                    {
                        pageNum = pageCount;
                    }
                    // 获取当前页码的记录
                    return context.Set<T>().OrderBy(keySelector).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to get '{0}'s.", typeof(T).Name));
                FileLogUtility.Error(ex);
                totalCount = 0;
                pageCount = 0;
                return new List<T>();
            }
        }

        /// <summary>
        /// 设置查询表达式来获取实体集合。
        /// </summary>
        /// <param name="predicate">查询条件表达式。</param>
        /// <returns>返回符合条件的实体集合。</returns>
        public virtual List<T> Get(Expression<Func<T, bool>> predicate)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    return context.Set<T>().Where(predicate).ToList(); // 过滤并获取实体集合。
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to get '{0}'s.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return new List<T>();
            }
        }

        /// <summary>
        /// 根据条件表达式，以分页方式来获取实体列表。
        /// </summary>
        /// <param name="predicate">条件表达式。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageNum">页码。</param>
        /// <param name="totalCount">总记录数。</param>
        /// <param name="pageCount">页面数。</param>
        /// <returns>当前页码的实体集合。</returns>
        public virtual List<T> GetPage(Expression<Func<T, bool>> predicate, int pageSize, ref int pageNum, out int totalCount, out int pageCount)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    totalCount = context.Set<T>().Where(predicate).Count(); // 过滤并获取总数

                    if (totalCount % pageSize == 0) // 获取页数
                    {
                        pageCount = totalCount / pageSize;
                    }
                    else
                    {
                        pageCount = totalCount / pageSize + 1;
                    }

                    if (pageNum <= 1) // 重置页码
                    {
                        pageNum = 1;
                    }
                    if (pageNum >= pageCount)
                    {
                        pageNum = pageCount;
                    }
                    // 获取指定页码的实体集合。
                    return context.Set<T>().Where(predicate).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to get '{0}'s.", typeof(T).Name));
                FileLogUtility.Error(ex);
                totalCount = 0;
                pageCount = 0;
                return new List<T>();
            }
        }

        /// <summary>
        /// 利用主键来获取唯一信息。
        /// </summary>
        /// <param name="keyValues">键值。</param>
        /// <returns>指定实体。</returns>
        public virtual T GetByKey(params object[] keyValues)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    return context.Set<T>().Find(keyValues);
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to get '{0}'.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 更新实体。
        /// </summary>
        /// <param name="entity">实体对象。</param>
        /// <returns>如果更新成功，返回true，否则返回false。</returns>
        public virtual bool Update(T entity)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    // 设置更新状态
                    context.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
                    // 更新数据库
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to update '{0}'.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 删除实体。
        /// </summary>
        /// <param name="entity">实体对象。</param>
        /// <returns>如果删除成功，返回true，否则返回false。</returns>
        public virtual bool Delete(T entity)
        {
            try
            {
                using (var context = NewDbContext())
                {
                    context.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex) // 将异常信息记录到调试日志。
            {
                FileLogUtility.Error(string.Format("Failed to delete '{0}'.", typeof(T).Name));
                FileLogUtility.Error(ex);
                return false;
            }
        }

        public abstract DbContext NewDbContext();
    }
}
