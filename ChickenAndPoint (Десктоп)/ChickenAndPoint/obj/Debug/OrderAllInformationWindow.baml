   M S B A M L   `   `   `  ����J  FChickenAndPoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ChickenAndPointChickenAndPoint  5   	   /    )ChickenAndPoint.OrderAllInformationWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:ChickenAndPoint     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$7  1Полная информация о заказе��5	   	   $
��Auto��5
   	   $	��650��6    � SizeToContent$ Height=�6#   $	��750��6:    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	    � AllowsTransparency .  � WindowStyle$
 None=�5   #   
������    MaterialDesignBody#���   5   	    � Icon$ 
/logo3.png��5   :   ��6	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension   � 	Resources 5       �� 
TargetType  
LabelStyle " MaterialDesignCaptionTextBlock	 �� BasedOn
 �� Property �� Value  
ValueStyle   MaterialDesignBody1TextBlock  InfoRowStyle��%j  &	       7  &	 �     7  &	 �     �� 5   
   " ��6   8	   .�5      �� 
 �@J�Opacity$	 0.7Q�6(   �� 5      
 � ��$ 0,0,10,2q�6'   5      �� 5   
   " ��6   8	   .�5      �� 
 � ��$ Medium"�6+   �� 5      
 � ��$
 Wrap=�6-   5      �� 5   
   " �6   .�5      �� 
 � ��$ 0,4,0,4q�6'   5      5      .��5       ��   PrimaryHueMidBrush#���  6   $��3q�6@    �� CornerRadius$ 8��6T     MaterialDesignPaper#���  6e   $��10q�6�    J� Effect 5!   
   3  -System.Windows.Media.Effects.DropShadowEffect  5"         
BlurRadius$ 10Q�6      ShadowDepth$ 3Q�6/      	Direction$	 270Q�6?      Color$ Black��6O      Opacity$	 0.2Q�6]   ._�5%   
   � -   5        $��20q�5%       � RowDefinitions 5&      �� 5'      $
��Auto �6    �� 5(      $
��Auto �6    �� 5)      $
��Auto �6    �� 5*      $
��Auto �6    �� 5+      $
��Auto �6    �� 5,      $��* �6    �� 5-      $
��Auto �6    �� 5.      $
��Auto �6    �� 5/      $
��Auto �6    .+�52      �� -   5         ��Name$ TitleTextBlock��52      $��0��60   $��Заказ №...��6=    $  MaterialDesignHeadline6TextBlock#��[ 6O   $��0,0,0,10q�6�   � 54      $��1��6   #��[ 6    $��0,0,0,10q�6F    � ColumnDefinitions 55      �� 56      $��* �6'   �� 57      $��* �6'   .+�59      �� $��0��6   .+�5:      �� #��[ 6    $��СТАТУС:��6D   �� 5;      -   5        $ StatusTextBlock��5;       #��[ 69   $��-��6]   #���  6f   $��15(�6�   5<      �� 5=      $��1��6   $��Right=�6-   .+�5>      �� #��[ 6    $&�� СОЗДАН / ОБНОВЛЕН:��6D   $��Right=�6^   �� 5?      #��[ 6    $��Right=�6D   �� 5@      -   5         ��Name$ CreationTimeRun��5@      $�-��67    / �� 6E   -   5        $ UpdateTimeRun��5@   I   $�-��6`   5A      5B      5C      #  MaterialDesignThemes.Wpf.Card  5E      $��2��6"   $��15q�6/   $��0,5,0,10q�6<      UniformCornerRadius$ 4Q�6N    (	 $MaterialDesignTextFieldBoxBackground#��� 	 6f   .��5F      �� .+�5G      �� #��[ 6    $��КЛИЕНТ:��6D   �� 5H      -   5        $ ClientNameTextBlock��5H       #��[ 6=   $��-��6a   $��0,0,0,5q�6j   � 5I       5J      �� 5K      $��* �6/   �� 5L      $��* �6/   .+�5N      �� $��0��6%   $��0,0,10,0q�65   .+�5O      �� #��[ 6(   $��ТЕЛЕФОН:��6L   �� 5P      -   5        $ ClientPhoneTextBlock��5P   (   #��[ 6F   $��-��6j   5Q      �� 5R      $��1��6%   .+�5S      �� #��[ 6(   $��ПОЧТА:��6L   �� 5T      -   5        $ ClientEmailTextBlock��5T   (   #��[ 6F   $��-��6j   5U      5V      5W      5X      � 5Z      $��3��6   #��[ 6     5[      �� 5\      $
��Auto �6'   �� 5]      $��* �6'   .+�5_      �� $��0��6   $��0,0,20,0q�6-   .+�5`      �� #��[ 6    $��ТИП ЗАКАЗА:��6D   �� 5a      -	   5        $ OrderTypeTextBlock��5a       #��[ 6<   $��-��6`   5b      �� 5c      -
   5         ��Name$ DeliveryAddressPanel��5c   -   $��1��6   $y�	Collapsed=�6K   .+�5d      �� #��[ 6    $"��АДРЕС ДОСТАВКИ:��6D   �� 5e      -   5        $ DeliveryAddressTextBlock��5e       #��[ 6B   $��-��6f   5f      5g      �� 5i      $��4��6   $��	0,10,0,10q�6%   �� 5k      -   5        $ LoadingStatusText��5k      $��5��63   $5��/Загрузка состава заказа...��6@   $��Center=�6b   $��Center=�6    $
  MaterialDesignSubtitle1TextBlock#��[
 6�     MaterialDesignBodyLight#���  6�   $y�Visible=�6  �� 5l      $��5��6   $
��Auto=�6(   $	��300��6K   .��5m      �� -   5         ��Name$ OrderItemsList��5m       � IsSharedSizeScope . ��5n      �� 5o       �� DataType/   )ChickenAndPoint.OrderDisplayItemViewModel"  6'   .R�5p      � $	��0,6q�6#    5q   "   �� 5r   &   $��* �67   $	��200��6A   �� 5s   &   $
��Auto �67    z� SharedSizeGroup$ QuantityCol��6D   $��40��6b   �� 5t   &   $
��Auto �67   $ PriceCol��6D   $��90��6_   �� 5u   &   $
��Auto �67   $ SumCol��6D   $	��100��6]   .+�5x   "   �� $��0��6,   #��[ 6V   $��Center=�6z   $
��Wrap=�6�   $��0,0,16,0q�6�   ��6<   �� *
DishName+6�   �� 5y   "   $��1��6,   #��[ 6k   $��Center=�6�   $��Right=�6�   $��0,0,16,0q�6�   ��6<   �� *
Quantity+ �� StringFormat$
 x{0}��6�   �� 5z   "   $��2��6,   #��[ 6u   $��Center=�6�   $��Right=�6�   $��0,0,16,0q�6�    J� Opacity$	 0.7Q�6�   $��13(�6�   ��6<   �� *PricePerItem+$ 
{0:N2} ₽��6�   �� 5{   "   $��3��6,   #��[ 6w   $��Center=�6�   $��Right=�6�   ��6<   �� *TotalItemPrice+$ 
{0:N2} ₽��6�   5|      5}      5      5�      �� 5�      $��6��6   $��0,10,0,5q�6%   � 5�      $��7��6   $��0,0,4,0q�6    .+�5�      �� $��ИТОГО:��6   #��[
 6*   $
��Left=�6d   $��Center=�6   �� 5�      -   5        $ TotalSumTextBlock��5�      #��[
 67   $
��Bold"�6q   $��Right=�6�   $��Center=�6�   $��- ₽��6�   5�      �� 5�      $��8��6    �� Orientation$ 
Horizontal=�6&   $��Right=�6?   $��0,15,0,0q�6[   .+�5�      �� -   5          ��Name$  CloseButton��5�      $��Закрыть��6.     MaterialDesignFlatButton#��[ 6@   ! �� IsCancel! . 6�   5�      5�      5�      5�      