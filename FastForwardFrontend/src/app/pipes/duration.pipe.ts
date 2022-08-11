import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration'
})
export class DurationPipe implements PipeTransform {

  transform(value: any, args?: any): any {

    var hours = Math.floor(value / 3600);
    var minutes = Math.floor((value % 3600) / 60);
    var seconds = Math.floor((value % 60));

    var minuteStr
    if (minutes < 10 ) {
      minuteStr = "0" + minutes;
    } else {
      minuteStr = minutes;
    }

    var secondStr;
    if (seconds < 10) {
      secondStr = "0" + seconds;
    } else {
      secondStr = seconds;
    }

    if (hours > 0) {
      return hours + ':' + minuteStr + ':' + secondStr;
    } else {
      return minuteStr + ':' + secondStr;
    }
  }

}
