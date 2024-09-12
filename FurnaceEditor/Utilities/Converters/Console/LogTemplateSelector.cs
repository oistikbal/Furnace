using System.Windows;
using System.Windows.Controls;
using FurnaceEditor.Utilities.Loggers;

namespace FurnaceEditor.Utilities.Converters.Console
{
    public class LogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InfoLogTemplate { get; set; }
        public DataTemplate ErrorLogTemplate { get; set; }
        public DataTemplate WarningLogTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LogMessage log)
            {
                return log.LogType switch
                {
                    LogType.Info => InfoLogTemplate,
                    LogType.Error => ErrorLogTemplate,
                    LogType.Warn => WarningLogTemplate,
                    _ => InfoLogTemplate
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
