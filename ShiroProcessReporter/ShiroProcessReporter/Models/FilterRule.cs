using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Models
{
    public partial class FilterRule : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilterRuleVaild))]
        [NotifyPropertyChangedFor(nameof(IsOriginalVaild))]
        public string _original;

        public FilterRule Clone()
        {
            return new FilterRule
            {
                Original = Original,
            };
        }

        public bool FilterRuleVaild => IsOriginalVaild;

        public bool IsOriginalVaild => ValidateOriginal();

        private bool ValidateOriginal()
        {
            if (string.IsNullOrWhiteSpace(Original))
            {
                return false;
            }

            try
            {
                var regex = new Regex(Original);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
