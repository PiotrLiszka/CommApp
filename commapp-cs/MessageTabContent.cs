using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace commappcs
{
    public class MessageTabContent
    {
        public TabItem Item {  get; init; }
        public Label NameLabel {  get; init; }
        public RichTextBox MessTextBox { get; init; }
        public Button TabCloseButton { get; init; }

        private readonly Grid tabGrid;

        public MessageTabContent(string friend)
        {
            tabGrid = new Grid();

            tabGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Name = "TabName",
                Width = new GridLength(3, GridUnitType.Star)
            });

            tabGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Name = "TabCloseButton",
                Width = new GridLength(1, GridUnitType.Star)
            });

            MessTextBox = new RichTextBox
            {
                Background = Brushes.LightGray,
                IsReadOnly = true,
                Document = new FlowDocument(),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(1),
                IsUndoEnabled = false
            };

            Item = new TabItem()
            {
                Name = friend,
                Content = MessTextBox,
                Header = tabGrid,
                MinWidth = 100
            };

            NameLabel = new Label()
            {
                Content = friend,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            TabCloseButton = new Button()
            {
                Name = friend,
                Content = "X",
                ClickMode = ClickMode.Release,
                Height = 20,
                Width = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent
            };

            NameLabel.SetValue(Grid.ColumnProperty, 0);
            TabCloseButton.SetValue(Grid.ColumnProperty, 1);

            tabGrid.Children.Add(NameLabel);
            tabGrid.Children.Add(TabCloseButton);
        }
    }
}