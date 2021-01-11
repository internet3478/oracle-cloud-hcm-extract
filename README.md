# oracle-cloud-hcm-extract
An UI that helps invoking an interface (process flow) in Oracle HCM Cloud, check its progress, display the results, and do rollbacks.<br /><br />

The application is created on VS C# WPF. No additional references nor packages are required to compile it.<br />
It will invoke the submit process web service to start the process, check it status every 2 seconds until the extract is done, and it will display the XML results.<br />
It can invoke sequentially up to 9 processes, this is useful for testing on different effective dates.<br />
Optionally, after all the processes are invoked, the processed can be rolled back too, starting from the last invokation up.<br />
The XML results are stored in the folder of your choice.<br /><br />

There is a pre-requisite, it needs a properties file (filename is "properties.txt").<br />
Fill it with the following key/values:<br />
environment=(pod).fa.(cloud-region).oraclecloud.com<br />
username=(username)<br />
password=(password)<br />
extract-name=(process-flow-name)<br />
effective-date=(yyyy-mm-dd)<br />
changes-only=(N|Y|ATTRIBUTE|ATTRIB_OLD|BLOCK|BLOCK_OLD)<br />
output-folder=(ex: c:\output-folder)<br />
custom-parameters-1=(ex: Effective Date=2021-01-01;Person Number=12345)<br />
custom-parameters-2=(additional-sequential-invokation)<br />
custom-parameters-3=<br />
custom-parameters-4=<br />
custom-parameters-5=<br />
custom-parameters-6=<br />
custom-parameters-7=<br />
custom-parameters-8=<br />
custom-parameters-9=<br />
ldg=<ldg><br />
