# PingTester
Console app to test ping to URL/IP address specified as input parameters. Hosts are pinged via ICMP pings with a period 100 ms, a timeout 300 ms. Results are serialized to xml file named as TestPing.xml and saved in the same folder as app was started up. The output file from previous PingTester run is deleted. The time period
that pings are send is specified as very first command line argument parameter. When the process of testing is finished statistics are saved to file or shown at the console depends on the setting (see Command Line Arguments section below).
Note : saving results to xml file is not implemented!
As the result of testing , the statistic is generated containing availability %, total amount of packets sent, min , max and average roundtrip of packets for each URL/IP address.

## Command Line Arguments

Command line arguments follow this rules :

1.position => duration (obligatory) in seconds

other positions  => URL or IP addresses to ping

last positions => additional parameters to output statistics = "console","textfile","xmlfile" prefixed with flag string "--output".

Each argument is devided by white space

Examples of correct input parameters: 

"60 seznam.cz google.com"

"90  seznam.cz google.com --output textfile"