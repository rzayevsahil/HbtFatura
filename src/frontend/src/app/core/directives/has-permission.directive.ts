import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy, effect } from '@angular/core';
import { PermissionService } from '../services/permission.service';

@Directive({
    selector: '[appHasPermission]',
    standalone: true
})
export class HasPermissionDirective implements OnInit {
    private requiredPermission?: string;
    private hasView = false;

    @Input() set appHasPermission(val: string) {
        this.requiredPermission = val;
        this.updateView();
    }

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        private permService: PermissionService
    ) { }

    ngOnInit() {
        this.updateView();
    }

    private updateView() {
        if (!this.requiredPermission) return;

        const isPermitted = this.permService.hasPermission(this.requiredPermission);

        if (isPermitted && !this.hasView) {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
        } else if (!isPermitted && this.hasView) {
            this.viewContainer.clear();
            this.hasView = false;
        }
    }
}
