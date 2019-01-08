import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChalallangesListComponent } from './chalallanges-list.component';

describe('ChalallangesListComponent', () => {
  let component: ChalallangesListComponent;
  let fixture: ComponentFixture<ChalallangesListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChalallangesListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChalallangesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
