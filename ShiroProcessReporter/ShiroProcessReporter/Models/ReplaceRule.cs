using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Models
{
    public partial class ReplaceRule : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReplaceRuleVaild))]
        [NotifyPropertyChangedFor(nameof(IsOriginalVaild))]
        public string _original;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ReplaceRuleVaild))]
        [NotifyPropertyChangedFor(nameof(IsOriginalVaild))]
        public string _replacement;

        public ReplaceRule Clone()
        {
            return new ReplaceRule
            {
                Original = Original,
                Replacement = Replacement,
            };
        }

        public bool ReplaceRuleVaild => IsOriginalVaild && !string.IsNullOrEmpty(Replacement);

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
