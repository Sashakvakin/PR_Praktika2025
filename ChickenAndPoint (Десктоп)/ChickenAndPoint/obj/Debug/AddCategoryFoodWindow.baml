   M S B A M L   `   `   `  ����J  FChickenAndPoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:ChickenAndPoint.AdminChickenAndPoint.Admin  5   	   1clr-namespace:ChickenAndPointChickenAndPoint  5   	   +    %ChickenAndPoint.AddCategoryFoodWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:ChickenAndPoint.Admin     5   	   4
convertersclr-namespace:ChickenAndPoint     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5        � Title$)  #Добавить Категорию��5
   	   $
��Auto��6$   $	��450��62    � SizeToContent$ Height=�6>    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	      MaterialDesignPaper#���   6     MaterialDesignBody#���  5   	    � Icon$ 
/logo3.png��5   :   ��6	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension   � 	Resources 5      /   )ChickenAndPoint.ImageUrlToBitmapConverter  ImageUrlConverter��%   &	         5   
   6/   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $,��&Новая категория блюд��6!    $  MaterialDesignSubtitle1TextBlock#��[ 6=   $��0,0,0,15q�6w   �� 5   
   -   5         ��Name$ CategoryNameTextBox��5      $��1��6    % !MaterialDesignFloatingHintTextBox#��[ 5      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$) #Название категории��5      $��0,0,0,15q�5      �� 5!   
   $��2��6   $��0,0,0,20q�6"   .+�5"      �� $,��&Иконка (опционально):��6    " MaterialDesignCaptionTextBlock#��[ 65   $��0,0,0,5q�6m   � 5#      	 � ColumnDefinitions	 5$      �� 5%      $
��Auto �6'   �� 5&      $��* �6'   .+�5)      �� $��0��6   $��64��6)   $��64��64   
 �� CornerRadius$
 4��6@     MaterialDesignDivider#���  6Q   $��0,0,15,0q�6�   ._�5*      � .+�5+      �� -   5         ��Name$ IconPreviewImage��5+       $��Uniform=�6:   $��4q�6L   '  !MaterialDesignThemes.Wpf.PackIcon  5,      -   5          Name$ IconPreviewPlaceholder��5,   2      Kind$ ImageOutline=�6R   $��32��6f   $��32��6q     MaterialDesignBodyLight#���  6}   $��Center=�6�   $��Center=�6�   5-      5.      �� 50      $��1��6   .+�51      �� -   5        $ CategoryIconUrlTextBox��51      #��[ 52      $ URL иконки��53      $��0,-5,0,5q�6J   54      �� 55      -   5         ��Name$ SelectIconButton��55        MaterialDesignFlatButton#��[ 56      $
��Left=�57      $��0,0,0,0q�58      .��5:      ��  �� Orientation$ 
Horizontal=�6%   .+�5;        $ ImageSearchOutline=�66   $��0,0,8,0q�6P   �� 5<      $)��#Выбрать / Загрузить��6(   5=      5>      5?      5@      5A      �� 5C   
   $��3��6   $ 
Horizontal=�6"   $��Right=�6;   .+�5D      �� -   5        $ 	AddButton��5D      $��Добавить��6(    	 MaterialDesignRaisedButton#��[	 6;   $	��100��5E      $��0,0,10,0q�6!   63   �� 5F      -   5        $ CancelButton��5F      $��Отмена��6+     
 MaterialDesignOutlinedButton#��[
 6<   $	��100��5G      6!   5H      5J      5K      