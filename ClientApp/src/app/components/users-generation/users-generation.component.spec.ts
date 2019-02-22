import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersGenerationComponent } from './users-generation.component';

describe('UsersGenerationComponent', () => {
  let component: UsersGenerationComponent;
  let fixture: ComponentFixture<UsersGenerationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsersGenerationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersGenerationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
