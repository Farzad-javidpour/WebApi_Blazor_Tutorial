using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pluralize.NET;
using System.Linq.Expressions;
using System.Reflection;
using Core.Domain.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Singularizin table name like Posts to Post or People to Person
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddSingularizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Pluralizing table name like Post to Posts or Person to People
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Pluralize(tableName));
            }
        }

        /// <summary>
        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="mustBeIdentity">Set to true if you want only "Identity" guid fields that named "Id"</param>
        public static void AddSequentialGuidForIdConvention(this ModelBuilder modelBuilder)
        {
            modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "NEWSEQUENTIALID()");
        }

        /// <summary>
        /// Set DefaultValueSql for sepecific property name and type
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="propertyName">Name of property wants to set DefaultValueSql for</param>
        /// <param name="propertyType">Type of property wants to set DefaultValueSql for </param>
        /// <param name="defaultValueSql">DefaultValueSql like "NEWSEQUENTIALID()"</param>
        public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, string propertyName,
            Type propertyType, string defaultValueSql)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                IMutableProperty property = entityType.GetProperties()
                    .SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                if (property != null && property.ClrType == propertyType)
                    property.SetDefaultValueSql(defaultValueSql);
            }
        }

        /// <summary>
        /// Set DeleteBehavior.Restrict by default for relations
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (IMutableForeignKey fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        /// <summary>
        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            MethodInfo applyGenericMethod = typeof(ModelBuilder).GetMethods()
                .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            foreach (Type type in types)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType &&
                        iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyConcreteMethod =
                            applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new object[] {Activator.CreateInstance(type)});
                    }
                }
            }
        }

        /// <summary>
        /// Dynamically register all Entities that inherit from specific BaseType
        /// </summary>
        /// <typeparam name="TBaseEntity">Base type that Entities inherit from this</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterAllEntities(this ModelBuilder modelBuilder,
            params Assembly[] assemblies)
        {
            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(IEntity).IsAssignableFrom(c));

            foreach (Type type in types)
            {
                modelBuilder.Entity(type);
                //Expression<Func<((type)(TBaseEntity)), bool>> isNotDeleted = i => i.IsDeleted == false;
                //modelBuilder.Entity(type).HasQueryFilter(isNotDeleted);
            }
        }

        public static void AddQueryFilters(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).ApplyQueryFilter<IEntity>(e => e.IsDeleted == false);
                }
            }
        }

        private static void ApplyQueryFilter<T>(this EntityTypeBuilder entityTypeBuilder,
            Expression<Func<T, bool>> expression)
        {
            var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
            var expressionFilter = ReplacingExpressionVisitor.Replace(
                expression.Parameters.Single(), parameterType, expression.Body);

            var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter();
            if (currentQueryFilter != null)
            {
                var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                    currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
                expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
            }

            var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
            entityTypeBuilder.HasQueryFilter(lambdaExpression);
        }
        
    }
}
