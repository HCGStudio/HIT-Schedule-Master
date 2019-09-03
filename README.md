# 哈工大课表大师  ~~（打安排御主）~~
本应用使用dotnet core编写，完全开放、开源

本应用仅适用于**哈尔滨工业大学**的课程导出，不兼容其他学校的系统

**作者不对使用本程序产生的任何后果负责**

![GPL3orLater](https://www.gnu.org/graphics/gplv3-or-later.png)

如果您在使用本程序的时候遇到了BUG或者有什么好的建议，欢迎您在Issuses中提出。

如果您对本程序进行了改进，欢迎PR！

[下载地址](https://github.com/HCG-Studio/HIT-Schedule-Master/releases)

[Wiki](https://github.com/HCGStudio/HIT-Schedule-Master/wiki)

## 主要功能

- 从校园网中下载个人课表或者班级推荐课表并将其转换为 iCalendar (RFC 5545) 格式以便导入到日历软件中
- 自动检查更新

## 一些特性

- 所有通过“个人课表”生成的日历，默认在课程开始前25分钟进行提醒
- 通过“班级推荐课表”生成的日历，默认不进行提醒

## 将来可能会实现的功能

- 在日历导出前能够删除课程

## 已知BUG

- 暂无

## Windows日历 如何导入

先使用您的**电子邮件账户**登录Windows日历程序，然后使用Windows日历打开生成的`ics`文件，自动显示导入。

根据提示，选择指定的日历即可完成导入。

![image1](https://github.com/HCGStudio/HIT-Schedule-Master/raw/master/images/image-1.png)

导入后，日历将与您登录的电子邮件账户同步，在移动端登录邮箱也会同步导入的日历。


## Google日历 如何导入

请参考[将活动导入到 Google 日历](https://support.google.com/calendar/answer/37118?hl=zh-Hans)进行导入。

在导入后，日历将于您的Gmail账户同步，在移动端登录Gmail账户，或者下载Google日历客户端就可以使用。

## iOS 如何导入

### 方法一

在Windows下使用Windows日历或者Google日历，在iOS的'邮件'应用中登录对应的电子邮件账户就可以导入日历到iOS设备。

### 方法二

在Windows下使用电子邮件将`ics`文件通过QQ传到手机，或者作为附件发送到iOS`邮件`应用中登录的账户，按照提示即可完成导入。

## Android 如何导入

### 方法一

在Windows下使用Windows日历或者Google日历，在您使用系统的`日历`应用中登录对应的电子邮件账户就可以导入日历到Android设备。

### 方法二

在Windows下使用电子邮件将`ics`文件通过QQ传到手机，选择使用`日历`打开。如果您的系统无法使用日历打开`ics`文件，建议您安装`Google 日历`（无需登录即可导入）或者其他支持的日历软件（欢迎在PR中提出）。
