   M S B A M L   `   `   `  ����Q  MUIShell.WpfShellPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulloLclr-namespace:UIShell.WpfShellPlugin.Utility;assembly=UIShell.WpfShellPluginUIShell.WpfShellPlugin.Utility  5        UIShell.RbacManagementPlugin<   6UIShell.RbacManagementPlugin.ChangePasswordUserControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      `utilityLclr-namespace:UIShell.WpfShellPlugin.Utility;assembly=UIShell.WpfShellPlugin     5      $	��400��5      $	��260��6+   .��5	      � $��0q�6   $��Center=�6     � ColumnDefinitions  5
   
   �� 5      $	��130 �6   �� 5      $	��150 �6    � RowDefinitions 5   
   �� 5      $��32 �6   �� 5      $��32 �6   �� 5      $��32 �6   �� 5      $��32 �6   �� 5      $��32 �6   �� 5      $��50 �6   .+�5   
   �� $��2q�6   $��0��6   $��0��6,   $��当前用户��6<   �� 5   
   -   5        $��CurrentUserTextBlock��5   <   $��2q�6   $��0��6   $��1��6,   6<   �� 5   
   $��2q�6   $��1��6   $��0��6,   $��密码��6<   G� 5   
   -   5        $ ��CurrentPasswordPasswordBox��5   >   $��2q�6   $��1��6!   $��1��6.   1   +UIShell.WpfShellPlugin.Utility.FocusUtility   AutoFocusAfterLoaded .     MoveFocusWhenEnterPressed .    SelectAllAfterFocused . 6�   �� 5   
   $��2q�6   $��2��6   $��0��6,   $��	新密码��6<   G� 5   
   -   5        $��NewPasswordPasswordBox��5   >   $��2q�6   $��2��6!   $��1��6.    .  . 6�   �� 5   
   $��2q�6   $��3��6   $��0��6,   $��确认密码��6<   G� 5    
   -   5        $ ��ConfirmPasswordPasswordBox��5    >   $��2q�6   $��3��6!   $��1��6.    .  . 6�   � 5"   
   $��Center=�6   $��4��6,   $��2��69     5#      �� 5$      $��75 �6#   �� 5%      $��75 �6#   .+�5'      �� -   5        $��OkButton��5'   =   $��2q�6   $��确定��6    $��0��6-   6M   �� 5(      -   5        $��NoButton��5(   M   $��2q�6   $��取消��6     �� IsCancel . $��1��6=   6]   5)      �� 5+   
   -   5        $��ErrorTextBlock��5+   f   $��Center=�6   $
��Wrap=�61   $��5��6E   $��2��6R   
���  ��6|   5,      5-      