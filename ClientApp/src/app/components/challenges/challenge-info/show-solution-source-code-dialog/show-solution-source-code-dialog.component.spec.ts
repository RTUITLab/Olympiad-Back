import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowSolutionSourceCodeDialogComponent } from './show-solution-source-code-dialog.component';

describe('ShowSolutionSourceCodeDialogComponent', () => {
  let component: ShowSolutionSourceCodeDialogComponent;
  let fixture: ComponentFixture<ShowSolutionSourceCodeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowSolutionSourceCodeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowSolutionSourceCodeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
