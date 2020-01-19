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

- 从校园网中下载个人课表或者班级推荐课表并将其转换为`iCalendar (RFC 5545) `格式以便导入到日历软件中
- 自动检查更新

## 为什么要使用本程序？

- 本程序导出的` iCalendar (RFC 5545) `格式受世界上几乎所有的现代操作系统支持，实现了真正的跨平台
- 由于日历一般为系统自带应用，因此UI往往与系统原生UI相同，并且系统的日历应用往往有优化。而且若不喜欢系统的日历应用，还可以使用第三方的日历应用。
- **本程序导出的课表默认在开课前进行提醒，能够有效防止忘课。**

## 一些特性

- 所有通过“个人课表”生成的日历，默认在课程开始前25分钟进行提醒
- 通过“班级推荐课表”生成的日历，默认不进行提醒

## 将来可能会实现的功能

- 在日历导出前能够删除课程
- 选择是否创建提醒以及提醒的时间

## 已知BUG

- 暂无

## CLI版本使用说明

输入'ls'即可获得所有可用命令。

## Windows日历 如何导入

**请注意，Windows版“日历”应用只能将事件导入到已经存在的日历中，这可能是不安全的，因此作者建议采用网页版Outlook，或者Google日历来完成事件导入。**

先使用您的**电子邮件账户**登录Windows日历程序，然后使用Windows日历打开生成的`ics`文件，自动显示导入。

根据提示，选择指定的日历即可完成导入。

![image1](./images/image-1.png)

导入后，日历将与您登录的电子邮件账户同步，在移动端登录邮箱也会同步导入的日历。

## Outlook日历如何导入

1. 首先登陆网页版[网页版Outlook日历](https://outlook.live.com/calendar/)进行导入。
2. 在左边栏中点击"添加日历"![image2](./images/image-3.png)
3. 在弹出的窗口中，如图示完成新建日历。![image3](./images/image-4.png)
4. 将ICS描述的事件导入到新建的日历中。![image4](./images/image-5.png)


## Google日历 如何导入

请参考[将活动导入到 Google 日历](https://support.google.com/calendar/answer/37118?hl=zh-Hans)进行导入。

在导入后，日历将于您的Gmail账户同步，在移动端登录Gmail账户，或者下载Google日历客户端就可以使用。

## iOS 如何导入

### 方法一

在Windows下使用Windows日历，Outlook日历或者Google日历，在iOS的'邮件'应用中登录对应的电子邮件账户就可以导入日历到iOS设备。

### 方法二

在Windows下使用电子邮件将`ics`文件通过QQ传到手机，或者作为附件发送到iOS`邮件`应用中登录的账户，按照提示即可完成导入。

## Android 如何导入

### 方法一

在Windows下使用Windows日历，Outlook日历或者Google日历，在您使用系统的`日历`应用中登录对应的电子邮件账户就可以导入日历到Android设备。

### 方法二

在Windows下使用电子邮件将`ics`文件通过QQ传到手机，选择使用`日历`打开。如果您的系统无法使用日历打开`ics`文件，建议您安装`Google 日历`（无需登录即可导入）或者其他支持的日历软件（欢迎在PR中提出）。
