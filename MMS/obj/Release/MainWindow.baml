   M S B A M L   `   `   `  ���� >  :MMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:MMSMMS      MMS.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:MMS       � Title$  
MainWindow��$	��500��$
��1512��$
��1512�� � 	Resources  �� 
TargetType   OptionsStackPanel �� Property �� Value  DescriptionTextBlock  	TitleLine  SeparatorLine�� %�  &	        &	 L     &	 �     &	 
    �� " ��.���  �@��Orientation$ 
Horizontal=���  � ��$ 4q��� " ��.���  � ��$ 0,0,4,0q���  � ��$ 70���� " ��.���  � ��$	 500����  � ��$ 2����  � ��
 �������  � ��$ Center=���  � ��$ 8q��� " ��.���  � ��$	 400����  � ��$ 1����  � ��
 �������  � ��$ Center=���  � ��$ 8q�.���� .���  � ColumnDefinitions �� �� ��  � RowDefinitions �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �.+��� .+��� $��Data input source��$��20(�$��Center=��� $6��0Data input can be live from the IED or generated��$��Center=��� -   $��InputSettingsRow1��$��1��.+��� $	��500��$��2��
�������$��Center=�$��8q��� -   $��InputRadioButtons�� �� Orientation$ 
Horizontal=�$��Center=�$��0,0,0,8q�.+�	� -   $��	LiveInput��$��0,0,20,0q�.��Live IED input	� -   $��GenInput��.��Generated input�� -   $��RefreshSettings��.+��� -   $��RefreshBool��$��Continually refresh input��$��Center=��� -   $��RefreshStackPanel��$y�	Collapsed=�$ 
Horizontal=�$��Center=�.+��� $��Refresh rate (ms):��$��0,0,8,0q��� -   $��RefreshRate��$	��400���� -	   $��InputSettingsRow2��$��2��.+��� #��[ � -
   $��LiveInputSettings��$y�	Collapsed=�$��Center=� �� $	��250 ��� $	��250 � �� �� .+��� $��Center=�.+��� $��IED COM port��$��8,24,4,4q�$��Center=��� -   $��SerialChooseIED�� �� SelectionMode$ Single=�
����������� .R��� $��4q����� �� $��2��$��0,24,0,0q�$��Center=�.+��� #��[  .+��� $��
Baud Rate:��#��[ �� -   $��BaudRate��$��38400��$��60���� #��[  .+��� $��
Data bits:��#��[ �� -   $��DataBits��$��8��$��60���� #��[  .+��� $��Parity:��#��[ �� -   $��ParityComboBox��	 �� SelectedIndex$	 0��.@��� .���� $
��None���� .���� $
��Even���� .���� $	��Odd���� #��[  .+��� $��
Stop Bits:��#��[ �� -   $��StopBitsComboBox��$	 0��.@��� .���� $��1���� .���� $��2���� #��[  .+��� $��Timeout:��#��[ �� -   $��Timeout��$	��400��$��60���� -   $��
WarningIED��$��3��$y�	Collapsed=�$��1��.+��� $5��/Warning - the IED must use a different COM port��
���  ��$��Center=��� -   $��GenInputSettings��$y�	Collapsed=�$��Center=�$��0,24,0,0q�.+��� -   $��
ZeroValues��$��0��$��4q��� -   $��P9_99��$��+9.99��$��4q��� -   $��N9_99��$��-9.99��$��4q��� -   $��AddressIsValue��$��Address = value��$��4q��� -   $��MaxValue��$��Maximum possible value��$��4q��� -   $��MinValue��$��Minimum possible value��$��4q��� $��1��.+��� $��PIM Clients��$��20(�$��Center=��� $��Center=�97If these devices are virtual simulators, this requires �� -   $/��)C:\Program Files (x86)\com0com\setupg.exe?�.�	com0com ports to be set up.�� $��	16,0,16,0q�$	��500��$��Center=�
 �� TextAlignment$
 Center=�$����Connecting PIM clients after the process has started will cause the entire program to crash, and will require it to be restarted.��$
��Wrap=��� -   $��ClientSettingsRow1��$��1��$��1��.+��� #��[ �� $ 
Horizontal=�$��Center=�$	��0,8q�.+�	� -   $��	OneClient��$��0,0,20,0q�.��
One client	� -   $��
TwoClients��$��0,0,20,0q�.��Two clients	� -   $��ThreeClients��$��0,0,20,0q�.��Three clients�� -   $��ClientSettingsRow2��$��1��$��2��.+��� #��[ �� -   $��
clientTabs��$y�	Collapsed=�$	��480��.@��� -    $��Client1��$��Client 1��$y�	Collapsed=�.���  �� $	��250 ��� $	��250 � �� �� .+��� $��Center=�.+��� $��Slave input COM port��$��8,4,4,4q�$��Center=��� -!   $��Client1Serial��$ Single=�
����������� .R��� $��4q����� �� $��2��$��Center=�.+��� #��[  .+��� $��
Baud Rate:��#��[ �� -"   $��Client1BaudRate��$��19200��$��60���� #��[  .+��� $��
Data bits:��#��[ �� -#   $��Client1DataBits��$��8��$��60���� #��[  .+��� $��Parity:��#��[ �� -$   $��Client1ParityComboBox��$	 0��.@��� .���� $
��None���� .���� $
��Even���� .���� $	��Odd���� #��[  .+��� $��
Stop Bits:��#��[ �� -%   $��Client1StopBitsComboBox��$	 0��.@��� .���� $��1���� .���� $��2���� -&   $��Client2��$��Client 2��$y�	Collapsed=�.���  �� $	��250 ��� $	��250 � �� �� .+��� $��Center=�.+��� $��Slave input COM port��$��8,4,4,4q�$��Center=��� -'   $��Client2Serial��$ Single=�
����������� .R��� $��4q����� �� $��2��$��Center=�.+��� #��[  .+��� $��
Baud Rate:��#��[ �� -(   $��Client2BaudRate��$��19200��$��60���� #��[  .+��� $��
Data bits:��#��[ �� -)   $��Client2DataBits��$��8��$��60���� #��[  .+��� $��Parity:��#��[ �� -*   $��Client2ParityComboBox��$	 0��.@��� .���� $
��None���� .���� $
��Even���� .���� $	��Odd���� #��[  .+��� $��
Stop Bits:��#��[ �� -+   $��Client2StopBitsComboBox��$	 0��.@��� .���� $��1���� .���� $��2���� -,   $��Client3��$��Client 3��$y�	Collapsed=�.���  �� $	��250 ��� $	��250 � �� �� .+��� $��Center=�.+��� $��Slave input COM port��$��8,4,4,4q�$��Center=��� --   $��Client3Serial��$ Single=�
����������� .R��� $��4q����� �� $��2��$��Center=�.+��� #��[  .+��� $��
Baud Rate:��#��[ �� -.   $��Client3BaudRate��$��19200��$��60���� #��[  .+��� $��
Data bits:��#��[ �� -/   $��Client3DataBits��$��8��$��60���� #��[  .+��� $��Parity:��#��[ �� -0   $��Client3ParityComboBox��$	 0��.@��� .���� $
��None���� .���� $
��Even���� .���� $	��Odd���� #��[  .+��� $��
Stop Bits:��#��[ �� -1   $��Client3StopBitsComboBox��$	 0��.@��� .���� $��1���� .���� $��2���� -2   $��WarningClient��
���  ��$��Center=�$y�	Collapsed=��� $��3��$��1��$ Vertical=�$��Center=�.+��� #��[ �� -3   $��PIMConnected��$=��7The PIMs are already connected and querying this device��$��4q��� $��4��$��1��$��0,0,0,8q�.+��� #��[ �� -4   $��Start��$��Start��{�.  ���#33cc57$	��100��$��Center=�$��	380,0,0,0q��� -5   $��Timer��$��30(�$��Center=��� -6   $��WarningTextBlock��$��3��$��5��$8��2Warning - only positive integer values are allowed��
���  ��$��Center=�$y�	Collapsed=��� $��Logger��$��20(�$��Center=�$��2���� -7   $��LogLocation��$��2��$��1��$��Center=�.+��� #��[ �� -8   $��SelectFileNameLocation��$ ��Change log output location��$	��240��$��Center=�$��20��$��0,0,0,8q��� -9   $��SelectedFolder��$��Center=�$0��*C:\\Users\\15028598738\\Documents\\MMS Log���� -:   $��Duration��$��2��$��2��.+��� #��[ �� -;   $��DurationCheckbox��$��Run for a set duration��$��Center=��� -<   $��
LoggerInfo��$��Center=�$y�	Collapsed=�.+��� $ 
Horizontal=�$��Center=�.+��� $ Vertical=�$��8,0,0,8q�.+��� $��Hours���� -=   $��Hours���� $	��4,0q�.+��� �� $��:���� $ Vertical=�$��0,0,8,8q�.+��� $��Minutes���� ->   $��Minutes��