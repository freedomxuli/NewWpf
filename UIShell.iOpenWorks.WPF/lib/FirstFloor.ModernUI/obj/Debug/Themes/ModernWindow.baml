   M S B A M L   `   `   `  ����N  JFirstFloor.ModernUI, Version=1.0.6.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:FirstFloor.ModernUIFirstFloor.ModernUI  5      [2clr-namespace:FirstFloor.ModernUI.Windows.Controls$FirstFloor.ModernUI.Windows.Controls  5      _4clr-namespace:FirstFloor.ModernUI.Windows.Navigation&FirstFloor.ModernUI.Windows.Navigation  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089b ^Microsoft.Windows.Shell, Version=3.5.41019.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35  �� MergedDictionaries �� Source7    1FirstFloor.ModernUI.Windows.Controls.ModernWindow �� 
TargetType �� Property   WindowBackgroundContent �� Value  DefaultFontFamily  DefaultFontSize  
WindowText  WindowBorder&   System.Windows.Media.TextOptions �� 
TargetType ��Name/  )Microsoft.Windows.Shell.SystemParameters2   Current �� Source	 �� Path  WindowBackgroundColor
 �� Color � 	Resources  Key �� 
TargetName �� TargetProperty	 �� To g� Duration �Name � RowDefinitions    BackgroundContent � ColumnDefinitions � Icon    
IconHeight � DataContext � Title  ModernWindowTitle    TitleFontSize �� Orientation*  $Microsoft.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome ��Name  SystemButton,  &Microsoft.Windows.Shell.SystemCommands   ShowSystemMenuCommand �� RelativeSource �� Mode �� AncestorType#   FirstFloor.ModernUI.Resources    Minimize! � ToolTip"   MinimizeWindowCommand#   Restore$   RestoreWindowCommand% � UseLayoutRounding&   Maximize'   MaximizeWindowCommand(   Close 	 SystemCloseButton)   CloseWindowCommand6   0FirstFloor.ModernUI.Windows.Controls.ModernFrame*  Name+    ContentLoader,   ContentLoader-   Source. �� Mode/   ResizeGripDirection0 �� Stretch1 �� Triggers2 P� Property3 P� Value 
 WindowBorderActive4 �� 
TargetName5 ]� 
Conditions6 �� Property7 �� Value8   CornerRadius9   GlassFrameThickness:   UseAeroCaptionButtons;   NonClientFrameEdges�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .modernui!clr-namespace:FirstFloor.ModernUI  5      ?controls2clr-namespace:FirstFloor.ModernUI.Windows.Controls  5      C
navigation4clr-namespace:FirstFloor.ModernUI.Windows.Navigation  5      Kshell?http://schemas.microsoft.com/winfx/2006/xaml/presentation/shell  5        5      �� 5	   
   $7 1/FirstFloor.ModernUI;component/Assets/Button.xaml?�6   �� 5
   
   $; 5/FirstFloor.ModernUI;component/Assets/Converters.xaml?�6   �� 5   
   $: 4/FirstFloor.ModernUI;component/Assets/TextBlock.xaml?�6   %  '         7  7  7  7  7  7 	 �� 5      "   6   .�5   
   ��  �@  BackgroundContent# �   6.   �� 5   
    � ��# �  6'   �� 5   
    � ��# �  6%   �� 5   
    � ��# �  6'   �� 5   
    � ��# �  6(   �� 5   
    � ��$	 800��6"   �� 5   
    � ��$	 640��6#   �� 5   
    � ��$	 320��6%   �� 5   
    � ��$	 320��6&   �� 5   
    �@�
ResizeMode$ CanResizeWithGrip=�6'   �� 5   
    �@�UseLayoutRounding . 6.   �� 5   
    �@ TextFormattingMode$ Display=�6;   �� 5   
    � �� 5      �� 5      "   6"   .R�5      �� $ WindowBorder��6   ��63   �� # Z $#	 WindowNonClientFrameThickness���5       �� 5!      WindowBorderBackground��#
 �  6N    5#      ���� 5$      BackgroundAnimation .��5%   "   �� $ WindowBorderBackground��61   $ Color�6`   # �  6�   $ 0:0:.6D�6�   5&      5'      ._�5)      �� #��z��6!   $��1q�6M   ._�5*      �� .u�5+   "   � $ 
LayoutRoot��6'    5,   &   �� 5-   *   $
��Auto �68   �� 5.   *   $��* �68   �� 5/   *   $
��Auto �68   .+�53   &   �� $��3��65   #��z 6F   � 56   &    57   *   �� 58   .   $��* �6?   �� 59   .   $	��128 �6?   .+�5=   *   � $��0��64   .+�5>   .   �� #��z 64   # z 6T   $	��8,0q�6�   ��6   �� 6�   �� 5?   .   #��z 68   $	��8,0q�6l   $��Center=�6y   8��  # z 5@   4   ��5?   W   �� 5@   4   5A   +   �� 5E   *   $��1��65   $ 
Horizontal=�6E   $	��Top=�6^    . .+�5F   .   �� $ SystemMenuButton��65   8�� ��6O   �� # Z ��5G   2   � 5H   6   $��13��6;   $��12��6F   $z�1,0,0,1,0,1W�6R   .+�5I   :   F� 4����    @B � z @B �     	= � z  	= � 	= @T� 	$��8��6c   $��10��6m   $��Center=�6y   $��Center=�6�   $��1��5J   �   ��6?   �� *
Foreground+ �� $ FindAncestor=�$ ButtonL�6�   5K   7   5N   /   �� 5O   .   #! Z  6�   8�� ��65   �� # Z" ��5P   2   � 5Q   6   $��13��6;   $��12��6F   $z�1,0,0,1,0,1W�6R   .+�5R   :   F� ����    ��[ � z ��[ 	$��8��6R   $��7��6\   $��Center=�6g   $��Center=�6�   $��2��5S   �   ��6?   �� *
Foreground+ �� $ FindAncestor=�$ ButtonL�6�   5T   7   5V   /   � 5W   .   $��1,0,1,0q�63   .+�5X   2   �� $ Restore��69   #! Z# 6�   8�� $y�	Collapsed=�6�   ��6J   �� # Z$ ��5Y   6   � 5Z   :   $��13��6?   $��12��6J   % . $z�1,0,0,1,.5,.5W�6o   .+�5[   >   F� X���Ѐ�     � z     � z ��[ �    ��- р�[ ��- �    �� р�[ �� р�[  z �     z 	$��8��6y   $��8��6�   $��Center=�6�   $��Center=�6�   $��1��5\   �   ��6C   �� *
Foreground+ �� $ FindAncestor=�$ ButtonL�6�   5]   ;   5_   3   �� 5`   2   $ Maximize��69   #! Z& 6�   8�� ��6K   �� # Z' ��5a   6   � 5b   :   $��13��6?   $��12��6J   .+�5c   >   F� +����    @B �@T� @B �@T�  z �     z 	$��9��6`   $��8��6j   $��Center=�6u   $��Center=�6�   $��2��5d   �   ��6C   �� *
Foreground+ �� $ FindAncestor=�$ ButtonL�6�   5e   ;   5g   3   5h   /   �� 5i   .   #! Z( 6�   8�� ��65   �� # Z) ��5j   2   � 5k   6   $��13��6;   $��12��6F   $z�1,0,0,1,0,1W�6R   .+�5l   :   F� +����        � z ��j � z     �    ��j 	$��8��6\   $��7��6f   $��Center=�6q   $��Center=�6�   $	��1.5��5m   �   ��6?   �� *
Foreground+ �� $ FindAncestor=�$ ButtonL�6�   5n   7   5p   /   5q   +   5r   '     5u   &   $* ContentFrame��6;   $��1��6Q   $��1q�6^   #, z+ 6�   - 6i   �� *ContentSource+ �� *TemplatedParent+$. TwoWay=�6�   � 5x   &   $ 
ResizeGrip��68   $��2��6+   
������ $y�	Collapsed=�6e   $��Right=�6|   $��Bottom=�6�   $/ BottomRight=�6�   .+�5y   *   F� $��12��6/   $��12��6:   $��1q�6F   #���  5z   /   $��1��5{   /   $
0 None=�5|   /   �����@B ��� ���- ��� �@KL ��� ���j ��� �@T� ��� ��ا ��� Ѐ� @T� р� �ا Ѐ�[ @T� р�[ �ا Ѐ�� @T� р�� �ا �@KL ��[ ���j ��[ �@T� ��[ ��ا ��[ Ѐ�[ @KL р�[ ��j Ѐ�� @KL р�� ��j �@T� �� ��ا �� Ѐ�� @B р�� ��- 5}   /   5~   '   5   #   5�      5�      5�      1 5�      P� 5�      2 �@�IsActive3 . . �5�      �� # � 
 6<    � ��5�      P� 5�      2 �@�WindowState$3 	Maximized=�69   . �5�      �� $4 Maximize��6%    � y�$ 	Collapsed=�6Q   �� 5�      $4 Restore��6%    � y�$ Visible=�6P   �� 5�      $4 
LayoutRoot��6%    � ��$ 7q�6O   5�      P� 5�      2 �@�WindowState$3 Normal=�69   . �5�      �� $4 Maximize��6%    � y�$ Visible=�6Q   �� 5�      $4 Restore��6%    � y�$ 	Collapsed=�6P   5�      ]� 5�      5 5�      �� 5�   "   6 �@�
ResizeMode$7 CanResizeWithGrip=�6B   �� 5�   "   6 �@�WindowState$7 Normal=�6C   ./�5�      �� $4 
ResizeGrip��6%    � y�$ Visible=�6S   5�      5�      5�      �� 5�   
    �@ WindowChrome 5�        5�      $8 0��6%   $9 1q�66   : .  $
; None=�6m   5�      5�      5�      