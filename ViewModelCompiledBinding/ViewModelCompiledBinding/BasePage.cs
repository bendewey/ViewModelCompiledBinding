using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ViewModelCompiledBinding
{
    public class BasePage : Page
    {
        protected void ConfigureCompiledBinding<TViewModel>(Expression<Func<TViewModel>> viewModel) where TViewModel : class
        {
            var vmSetter = ExtractSetterFromLambda(viewModel);
            DataContextChanged += (s, e) =>
            {
                vmSetter.Invoke(this, new object[] { DataContext as TViewModel });
            };
        }

        private static MethodInfo ExtractSetterFromLambda(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("The expression body is not a MemberExpression.", "expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("The member expression is not a property.", "expression");

            var setMethod = property.SetMethod;
            if (setMethod == null)
                throw new ArgumentException("The expression property does not have a setter.", "expression");

            return setMethod;
        }
    }
}
