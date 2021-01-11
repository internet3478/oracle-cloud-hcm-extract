# oracle-cloud-hcm-extract
An UI that helps invoking an interface (process flow) in Oracle HCM Cloud, check its progress, display the results, and do rollbacks.

The application is created on VS C# WPF. No additional references nor packages are required to compile it.
It will invoke the submit process web service to start the process, check it status every 2 seconds until the extract is done, and it will display the XML results.
It can invoke sequentially up to 9 processes, this is useful for testing on different effective dates.
Optionally, after all the processes are invoked, the processed can be rolled back too, starting from the last invokation up.
The XML results are stored in the folder of your choice.

There is a pre-requisite, it needs a properties file (filename is "properties.txt").
Fill it with the following key/values:
environment=<pod>.fa.<cloud-region>.oraclecloud.com
username=<username>
password=<password>
extract-name=<process-flow-name>
effective-date=<yyyy-mm-dd>
changes-only=<N|Y|ATTRIBUTE|ATTRIBUTE_OLD|BLOCK|BLOCK_OLD>
output-folder=<ex: c:\output-folder>
custom-parameters-1=<ex: Effective Date=2021-01-01;Person Number=12345>
custom-parameters-2=<additional-sequential-invokation>
custom-parameters-3=
custom-parameters-4=
custom-parameters-5=
custom-parameters-6=
custom-parameters-7=
custom-parameters-8=
custom-parameters-9=
ldg=<ldg>
