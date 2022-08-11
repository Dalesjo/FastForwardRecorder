import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'secondsLeft'
})
export class SecondsLeftPipe implements PipeTransform {

  transform(value: number, args?: any): number {
    return Math.floor((value % 60));;
  }

}
