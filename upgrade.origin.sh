#!/bin/bash

set -e
set -u

if [ $# -lt 2 ]; then
        echo "Usage: $0 -daro <URL_or_ZIP_file>"
        exit 1
fi


URL=$2
DW_TMP=/var/tmp/update/$(cat /dev/urandom | tr -cd 'a-f0-9' | head -c 32) # Generate temp dir name
SQ_TMP=$DW_TMP/volume
OPT_MNT=$DW_TMP/opt
ETC_TMP=$DW_TMP/tmp/etc
VAR_TMP=$DW_TMP/var
DW_FILE=$URL # Will be changed to path to dowloaded file if ran with parameter -d

mkdir -p $DW_TMP
mkdir -p $SQ_TMP

cd $DW_TMP

download() {
	wget $URL
	DW_FILE=$DW_TMP/$(basename $URL)
}

unpack() {
	unzip $DW_FILE -d $SQ_TMP
}

upgrade_app() {
	cd $SQ_TMP
	cd $(ls | head -n 1)

	echo ""
	mkdir -p $OPT_MNT
	mount *opt* $OPT_MNT
	APPS=$(ls $OPT_MNT)
	umount $OPT_MNT

	echo -e "Found apps:\n\n$APPS"
	echo ""

	echo "Upgrading APP ETC ..."
	mkdir -p $ETC_TMP
	tar -C $ETC_TMP -xJf *etc.rw*
	for app in $APPS; do
		if [ -d $ETC_TMP/$app ]; then
			echo "Copying $app etc files ..."
			cp -R $ETC_TMP/$app /etc.rw
		fi
	done
	rm -R $ETC_TMP

	echo "Upgrading APP RESOURCES ..."
	mkdir -p $VAR_TMP
	tar -C $VAR_TMP -xJf *var*
	if [ -d $VAR_TMP/opt ]; then
		cp -R $VAR_TMP/opt /var
		rm -R $VAR_TMP
	else
		echo "ERROR while upgrading app resources - RESOURCES NOT FOUND!"
	fi

	echo "Upgrading APP BINARIES ..."
	emb-upgraderofs *opt*
}

upgrade_os() {
	cd $SQ_TMP
        cd $(ls | head -n 1)

	echo "Upgrading KONOS OS ..."
	emb-upgraderofs *rootfs*
}

clean() {
	echo "Cleaning ..."
	rm -R $DW_TMP
}

run() {
	mkdir -p $DW_TMP
	mkdir -p $SQ_TMP

	cd $DW_TMP

	[ $DOWNLOAD == "yes" ] && download
	unpack
	[ $UPGRADE_APP == "yes" ] && upgrade_app
	[ $UPGRADE_OS == "yes" ] && upgrade_os
	clean

	echo "DONE! Reboot your system."
}

while getopts ":daro" opt; do
        case $opt in
        d)
                DOWNLOAD="yes"
                ;;
        a)
                UPGRADE_APP="yes"
                ;;
        r)
                UPGRADE_ROOT="yes"
                ;;
        o)
                UPGRADE_OS="yes"
                ;;
        \?)
                echo "Invalid option: -$OPTARG" >&2
                ;;
        esac
done

: ${DOWNLOAD:="no"}
: ${UPGRADE_ROOT:="no"}
: ${UPGRADE_APP:="no"}
: ${UPGRADE_OS:="no"}

run
