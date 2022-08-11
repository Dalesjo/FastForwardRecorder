import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { DateToSecondsPipe } from './pipes/date-to-seconds.pipe';
import { DaysLeftPipe } from './pipes/days-left.pipe';
import { DurationPipe } from './pipes/duration.pipe';
import { FileSizePipe } from './pipes/filesize.pipe';
import { HoursLeftPipe } from './pipes/hours-left.pipe';
import { MinutesLeftPipe } from './pipes/minutes-left.pipe';
import { SecondsLeftPipe } from './pipes/seconds-left.pipe';
import { ZeroPadPipe } from './pipes/zero-pad.pipe';

@NgModule({
  declarations: [
    AppComponent,
    DaysLeftPipe,
    HoursLeftPipe,
    MinutesLeftPipe,
    SecondsLeftPipe,
    DateToSecondsPipe,
    ZeroPadPipe,
    DurationPipe,
    FileSizePipe

  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
