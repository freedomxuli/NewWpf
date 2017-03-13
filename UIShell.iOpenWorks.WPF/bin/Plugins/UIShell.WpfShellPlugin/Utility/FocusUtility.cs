using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UIShell.WpfShellPlugin.Utility
{
    public static class FocusUtility
    {
        public static readonly DependencyProperty AutoFocusAfterLoadedProperty =
            DependencyProperty.RegisterAttached("AutoFocusAfterLoaded", typeof(bool), typeof(FrameworkElement), new UIPropertyMetadata(null));

        public static bool GetAutoFocusAfterLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoFocusAfterLoadedProperty);
        }

        public static void SetAutoFocusAfterLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoFocusAfterLoadedProperty, value);
            if(value)
            {
                (obj as FrameworkElement).Loaded += (sender, e) => { (obj as FrameworkElement).Focus(); };
            }
        }

        public static readonly DependencyProperty MoveFocusWhenEnterPressedProperty =
            DependencyProperty.RegisterAttached("MoveFocusWhenEnterPressed", typeof(bool), typeof(FrameworkElement), new UIPropertyMetadata(null));

        public static bool GetMoveFocusWhenEnterPressed(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoFocusAfterLoadedProperty);
        }

        public static void SetMoveFocusWhenEnterPressed(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoFocusAfterLoadedProperty, value);
            if (value)
            {
                var element = obj as FrameworkElement;
                element.KeyDown += (sender, e) => 
                {
                    if (e.Key == Key.Enter)
                    {
                        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);

                        // Gets the element with keyboard focus.
                        UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

                        // Change keyboard focus.
                        if (elementWithFocus != null)
                        {
                            var oldElement = elementWithFocus;

                            elementWithFocus.MoveFocus(request);

                            elementWithFocus = Keyboard.FocusedElement as UIElement;

                            if(element is ComboBox && elementWithFocus is ToggleButton)
                            {
                                elementWithFocus.MoveFocus(request);
                                elementWithFocus = Keyboard.FocusedElement as UIElement;
                            }

                            if (elementWithFocus is Button)
                            {
                                var button = elementWithFocus as Button;
                                if(button.Command != null)
                                {
                                    button.Command.Execute(button.CommandParameter);
                                }
                                else
                                {
                                    elementWithFocus.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, elementWithFocus));
                                }
                            }
                            else if(elementWithFocus is ComboBox)
                            {
                                var comboBox = elementWithFocus as ComboBox;
                                if(!comboBox.IsEditable)
                                {
                                    (elementWithFocus as ComboBox).IsDropDownOpen = true;
                                }
                            }
                        }
                        e.Handled = true;
                    }
                };
            }
        }

        public static readonly DependencyProperty SelectAllAfterFocusedProperty =
            DependencyProperty.RegisterAttached("SelectAllAfterFocused", typeof(bool), typeof(FrameworkElement), new UIPropertyMetadata(null));

        public static bool GetSelectAllAfterFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllAfterFocusedProperty);
        }

        public static void SetSelectAllAfterFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllAfterFocusedProperty, value);
            if (value)
            {
                if(obj is TextBox)
                {
                    (obj as FrameworkElement).GotFocus += (sender, e) => { (obj as TextBox).SelectAll(); };
                }
                else if (obj is PasswordBox)
                {
                    (obj as FrameworkElement).GotFocus += (sender, e) => { (obj as PasswordBox).SelectAll(); };
                }
            }
        }
    }
}
