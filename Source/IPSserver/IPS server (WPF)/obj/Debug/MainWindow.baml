   M S B A M L   `   `   `  ����  IPS server (WPF)!    IPS_server__WPF_.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  WiLocator Server��5   	   $	��600��6"   $	��800��6/    � 
ResizeMode$ CanResizeWithGrip=�6;    J� SnapsToDevicePixels .  � SizeToContent$ Width=�6u   .��5      � .+�5   
   �� -   5         ��Name$ BListen��5      $��Start Listening��6"   $��	67,0,0,33q�6<   
 � Tag$  ��6e   $��22��6l   $��Bottom=�6x   $
��Left=�6�   $��90��6�   �� 5   
   -   5         ��Name$ TBPort��5      $��6,0,0,33q�6"   $
��5000��64   $  ��6@    � Cursor$ IBeam��6G    �� 	MaxLength$ 5��6V   	 �� MaxLines$	 1��6d   &   System.Windows.Media.TextOptions
   TextFormattingMode$
 Display=�6q   $��23��6�   $��Bottom=�6�   $
��Left=�6�   $��56��6�   �� 5   
   -   5         ��Name$ Status��5      $��29��6$   $��Bottom=�60   .@�5	      �� -   5         ��Name$ STStatus��5	      $��Not Listening��6&   $	��311��6>   $��10(�6J   }�. $  ��6l   $��Italic$�6s   $
 Display=�6�   5
      �� 5   
   -   5        $ BSave��5      $
��Save��6    $��
162,0,0,33q�6/   $��22��6W   $��Bottom=�6c   $
��Left=�6~   $��46��6�   �� 5   
   -   5        $ BLoad��5      $
��Load��6    $
��Left=�6/   $��
213,0,0,34q�6J   $��46��6^   $��22��6}   $��Bottom=�6�   �� 5   
   -   5         ��Name$	 Map��5      $��10,10,11,61q�6   $��2C.png��62    . 6B   �� 5   
   -   5        $ TBPrecision��5      $��
264,0,0,33q�6'    �� TextWrapping$
 Wrap=�6;   $��5��6O   $
��Left=�6X   $��33��6s   $��23��6~   $��Bottom=�6�   �� 5   
   -	   5        $ BSetprecision��5      $	��Set��6(   $
��Left=�66   $��
302,0,0,35q�6Q   $��35��6�   $��22��6�   $��Bottom=�6�   5      5      