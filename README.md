# antenna-analyzer
This a a fork/clone of the famous K6BEZ antenna analyzer.

I have build the hardware and still testing the software.

## Arduino
Directory DDS_sweeper contains code from K6BEZ

Directory DDS_sweeper2 is DK2JK version that works with the PC software VNA (vna.exe)

###Different versions/different outputs


DDS_sweeper.ino output


(Frequency,VSWR)


7100000.00,1155

dds_sweeper2_2 output

(Frequency,0,VSWR,FWD,REV)

24000000.00, 0, 1005.56622, 360, 1


## PC Software

The PC Software (https://github.com/billygr/antenna-analyzer/tree/master/VNA_program_source) is from DG7EAO (http://www.dg7eao.de/arduino/antennen-analysator).
I did a cleanup from the original source, make sure that it compiles clean using Visual Studio Community 2015 (minor warnings to be solved), switched to en locale, and it works ONLY with dds_sweeper2_2
