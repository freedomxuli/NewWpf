   M S B A M L   `   `   `  ����W  SUIShell.RbacManagementPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:UIShell.RbacManagementPlugin.ViewModels'UIShell.RbacManagementPlugin.ViewModels  5      Q MUIShell.WpfShellPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulloLclr-namespace:UIShell.WpfShellPlugin.Utility;assembly=UIShell.WpfShellPluginUIShell.WpfShellPlugin.Utility 5      ;    5UIShell.RbacManagementPlugin.AddOrEditUserUserControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Glocal5clr-namespace:UIShell.RbacManagementPlugin.ViewModels     5      `utilityLclr-namespace:UIShell.WpfShellPlugin.Utility;assembly=UIShell.WpfShellPlugin     5      -   5        $��AddUserUserControl��5        � 	Resources  5
      D   >UIShell.RbacManagementPlugin.ViewModels.AddOrEditUserViewModel   AddOrEditUserViewModel��%   &	          5   
   6H   5       � DataContext 5      �� 5   
    �� ResourceKey$ AddOrEditUserViewModel��6   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��100 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��36 �6   �� 5      $��36 �6   �� 5      $��36 �6   �� 5      $��36 �6   �� 5      $��36 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6$   $��	姓名：��61   �� 5   
   -   5        $��DisplayNameTextBox��5   /   $��1��6   $��0��6"   $��28��6�   $	��Top=�6�   1  +UIShell.WpfShellPlugin.Utility.FocusUtility   AutoFocusAfterLoaded .     MoveFocusWhenEnterPressed .    SelectAllAfterFocused . ��6I   �� *UserViewModel.DisplayName+ �� Mode$ TwoWay=�6  �� 5!   
   $��0��6   $��1��6$   $��登录名称：��61   �� 5"   
   -   5        $��NameTextBox��5"   /   $��1��6   $��1��6"   $��28��6B   $	��Top=�6    .  . ��6N   �� *UserViewModel.Name+$ TwoWay=�6�   �� 5$   
   $��0��6   $��2��6$   $��	密码：��61   �� 5%   
   -   5        $��PasswordTextBox��5%   /   $��1��6   $��2��6"   $��28��6F   $	��Top=�6�    .  . ��6R   �� *UserViewModel.Password+$ TwoWay=�6�   �� 5'   
   $��0��6   $��3��6$   $��	角色：��61   �� 5(   
   -   5        $��RoleComboBox��5(   0   $��1��6   $��3��6#   $��28��6D   $	��Top=�6P   	 �� SelectedValuePath$	 Id��6h   
 �� DisplayMemberPath$

 Name��6    .  �� SelectedValue 6�   �� *UserViewModel.RoleId+$ TwoWay=���6�   �� *Roles+6�   �� 5*   
    �� Orientation$ 
Horizontal=�6   $��0��6.   $��4��6>   $��2��6K   $��Center=�6_   .+�5+      �� -   5        $��	YesButton��5+   +   $��80��6   $��2q�6    ��6<   �� *	OkCommand+��6Z   ��  �� ElementName$ AddUserUserControl��.��5,      ]� .+�5-      �� $��15��6   $��15��6'   $
��Fill=�63   $��0,0,2,0q�6B   $<��6/UIShell.RbacManagementPlugin;component/Assets/yes.png��6S   �� 5.      $��center=�6    $��保存��6;   5/      50      �� 51      -   5        $��NoButton��51   +   $��80��6   $��2q�6     �� IsCancel . ��6K   �� *CancelCommand+��6m   �� $ AddUserUserControl��.��52      ]� .+�53      �� $��15��6   $��15��6'   $
��Fill=�63   $��0,0,2,0q�6B   $;��5/UIShell.RbacManagementPlugin;component/Assets/no.png��6S   �� 54      $��center=�6    $��取消��6;   55      56      57      �� 59   
   $��0��6   $��5��6$   $��2��61   
���  ����6B   �� *Error+6Y   5:      5;      