   M S B A M L   `   `   `  ����J  FChickenAndPoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ChickenAndPointChickenAndPoint  5   	   )    #ChickenAndPoint.SotrydnikMainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:ChickenAndPoint     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$7  1Курочка и точка - Сотрудник��5	   	   $	��600��6-   $	��900��6:    � WindowStartupLocation$ CenterScreen=�5
   	      MaterialDesignBody#���   5   	     MaterialDesignPaper#���  5   	   $��Regular"�5   	   $��13(�5   	    � Icon$ 
/logo3.png��5   :   ��6	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6     PrimaryHueMidBrush#���  6%   $	��200��6W   .+�5      �� -   5         ��Name$ ProfileButton��5        MaterialDesignFlatButton#��[ 6,      PrimaryHueMidForegroundBrush#���  6^   $��50��6�   $��10q�6�   
 � Tag$ Профиль��6�   .��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Account=�6.   $��Center=�6=   $��0,0,10,0q�6X   �� 5      $��Профиль��6    $��Center=�6/   5      5      �� 5      -   5        $ OrdersButton��5      #��[ 6+   #���  6]   $��50��6�   $��10q�6�   $ Заказы��6�   .��5      �� $ 
Horizontal=�6   .+�5        $ ClipboardList=�6.   $��Center=�6C   $��0,0,10,0q�6^   �� 5       $��Заказы��6    $��Center=�6.   5!      5"      5#      � 5%   
   $��1��6   .+�5&      �� -   5         ��Name$ ProfilePanel��5&      $y�	Collapsed=�6/   $��20q�6F   .+�5'      #  MaterialDesignThemes.Wpf.Card  $��20q�6&   $��10q�63   	   UniformCornerRadius$	 10Q�6?   .��5(      �� .+�5)      �� $)��#Профиль Сотрудника��6$    $  MaterialDesignHeadline6TextBlock#��[ 6>   $��0,0,0,15q�6x   �� 5*      $��Полное имя:��6$   $
��Bold"�67   �� 5+      -   5        
 ��Name$
 FullNameTextBlock��5+   $   $��-��6?   $��0,0,0,10q�6H   �� 5,      $"��Номер телефона:��6$   $
��Bold"�6;   �� 5-      -   5        $
 PhoneTextBlock��5-   $   $��-��6<   $��0,0,0,10q�6E   5.      5/      �� 50      -   5        $ LogoutButton��50      $��
Выход��6/     MaterialDesignRaisedButton#��[ 6?   $
��Left=�6s   $��10,20,10,10q�6�   $	��150��6�    ( $MaterialDesignTextFieldBoxBackground#���  6�   #���   6�   #���  6%  $��1q�6X  6l  51      � 53      -   5         �Name$ OrdersPanel��53      $y�	Collapsed=�6(   $��10q�6?   .+�54      �� .+�55      �� $,��&Текущие заказы (Тест)��6    #��[ 6=   $��10q�6w   �� 56      -   5        $
 OrdersStatusText��56       $(��"Загрузка заказов...��6:   $��Center=�6U   $��20q�6r   $y�	Collapsed=�6~   &   System.Windows.Controls.DataGrid  57      -	   5          Name$ OrdersDataGrid��57         AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly . $��10q�5<      -  'MaterialDesignThemes.Wpf.DataGridAssist   CellPadding$ 13 8 8 8q�5=         ColumnHeaderPadding$ 8q�5>      ��5;      �� *
OrdersList+   Columns 5?      1  +MaterialDesignThemes.Wpf.DataGridTextColumn  5@      ,  &System.Windows.Controls.DataGridColumn   Header$ ID Клиента��6@      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6r   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 6T   �� *IdКлиента+6r     5A      $ ID Статуса��6@   $
 Auto 6r    6T   �� *IdСтатуса+6r     5B      $ ID Типа��6@   $
 Auto 6l    6Q   �� *
IdТипа+6l     5C      $! Адрес доставки��6@   $ * 6z    6X   �� *АдресДоставки+6z     5D      $ 
Сумма��6@   $
 Auto 6�    6O   �� *ИтоговаяСумма+ �� StringFormat$ C��6�     5E      $! Время создания��6@   $
 Auto 6�    6X   �� *ВремяСоздания+$ dd.MM.yyyy HH:mm:ss��6�     5F      $% Время обновления��6@   $
 Auto 6�    6Z   �� * ВремяОбновления+$ dd.MM.yyyy HH:mm:ss��6�   5H      5I      5J      5K      5L      5M      