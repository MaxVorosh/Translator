   M S B A M L   `   `   `  ????E  ATranslator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Translator
Translator  5   	       Translator.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Translator     5   	     ? Title$  
MainWindow??5   	   $	??550??6   $	??800??6)   
??????? ? 
ResizeMode$ NoResize=?6K   .??5	      ?  ? ColumnDefinitions 5
   
   ?? 5      ?? 5       ? RowDefinitions 5   
   ?? 5      $
??0.1* ?6   ?? 5      $
??0.8* ?6   ?? 5      $
??0.1* ?6   .+?5   
   ?? -   5         ??Name$ TextToTranslate??5   ;   $??0??5      $??1??6"   $	??350??6/   $	??400??6;   $??Center=?6H   $??Center=?6e    ?? TextWrapping$
 Wrap=?5       ?? AcceptsReturn$
 True??6&   6;   ?? 5   
   -   5        $ TranslatedText??5   $   $??1??5      $??1??6"   $	??350??6/   $	??400??6;   $??Center=?6H   $??Center=?6e    ?? 
IsReadOnly$
 True??5      6$   ?? 5   
   $??0??6   $??0??6!   $??Python??61   $	??100??6?   $??Center=?6K   $??Bottom=?5       ?? TextAlignment$ Center=?6/   ?? 5   
   $??1??6   $??0??6$   $??C#??61   $	??100??6;   $??Center=?6G   $??Bottom=?5      $ Center=?6/   ?? 5   
   -   5        $??0??5      $??2??6!   $	??100??6.   $??30??6:   $??Clear??6F   $??Center=?6V   $??Center=?5      6,   ?? 5   
   -   5        $??2??5      $??1??6   $	??100??6.   $??30??6:   $??	Translate??6F   $??Center=?6Z   $??Center=?5      6,   5      5       