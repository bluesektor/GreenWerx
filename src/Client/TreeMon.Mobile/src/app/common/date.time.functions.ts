
export class DateTimeFunctions {

    public static combine (date: any, time: any ): Date {
        console.log('DateTimeFunctions.combine time type of:' , typeof time);

        if (typeof date === 'string' && typeof time === 'string') {
            return DateTimeFunctions.combineByString(date, time);
        }

        if (typeof date === 'object' && typeof time === 'object') {
            const ampm =  time.ampm.text; // .value
            let hour = time.hour.value;
            if ( time.hour.value < 10) {
                hour = + '0' + time.hour.text;
            }
            if ( ampm === 'PM' && time.hour.value < 12  ) {
                hour =   time.hour.value  + 12;
            }
            const minute = time.minute.text;
            const day = date.day.text;
            const month  = date.month.value; // because text is non numeric
            const year = date.year.text;

            const dateString = year + '-' + month + '-' + day + 'T' + hour + ':' + minute;
            console.log('DateTimeFunctions.combine objD objT dateString:', dateString);
            const fullDate = new Date(dateString );
            console.log('DateTimeFunctions.combine objD objT fullDate:', fullDate);
            return fullDate;
        }

        if (typeof date === 'string' && typeof time === 'object') {
            const ampm =  time.ampm.text; // .value
            let hour = time.hour.value;
            if ( time.hour.value < 10) {
                hour = + '0' + time.hour.text;
            }
            if ( ampm === 'PM' && time.hour.value < 12  ) {
                hour =   time.hour.value  + 12;
            }
            const minute = time.minute.text;

            let dateParts = date.split('T') as any[];
            dateParts = dateParts[0].split('-') as any[];
            const day = dateParts[2];
            const month  = dateParts[1];
            const year = dateParts[0];

            const dateString = year + '-' + month + '-' + day + 'T' + hour + ':' + minute;
            console.log('DateTimeFunctions.combine stringD objT dateString:', dateString);
            const fullDate = new Date(dateString );
            console.log('DateTimeFunctions.combine stringD objT fullDate:', fullDate);
            return fullDate;
        }

        if (typeof date === 'object' && typeof time === 'string') {
            let timeParts =  time .split('T');
            timeParts =  timeParts[1].split(':');
            const hour =  timeParts[0];
            const minute =  timeParts[1];

            const day = date.day.text;
            const month  = date.month.value; // because text is non numeric
            const year = date.year.text;

            const dateString = year + '-' + month + '-' + day + 'T' + hour + ':' + minute;
            console.log('DateTimeFunctions.combine objD stringT dateString:', dateString);
            const fullDate = new Date(dateString );
            console.log('DateTimeFunctions.combine objD stringT fullDate:', fullDate);
            return fullDate;
        }
    }

    public static combineByString (date: string, time: string ): Date {

        console.log('DateTimeFunctions.combineByString date:', date);
        console.log('DateTimeFunctions.combineByString time:', time);
        //  2018-10-20T19:34:10.669Z
        // if($scope.sdate && $scope.stime) {
        let dateParts = date.split('T') as any[];
        dateParts = dateParts[0].split('-') as any[];
        let timeParts =  time .split('T');
        timeParts =  timeParts[1].split(':');
        console.log('DateTimeFunctions.combineByString dateParts:', dateParts);
        console.log('DateTimeFunctions.combineByString timeParts:', timeParts);
        if (!dateParts && !timeParts) { return null; }

        const dateString = dateParts[0] + '-' +  dateParts[1] + '-' + dateParts[2] + 'T' + timeParts[0] + ':' + timeParts[1];
        const fullDate = new Date(dateString );
        console.log('DateTimeFunctions.combineByString fullDate:', fullDate);
        return fullDate;
    }

}
